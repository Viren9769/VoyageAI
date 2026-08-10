import {
    ChangeDetectionStrategy,
    ChangeDetectorRef,
    Component,
    DestroyRef,
    OnInit,
    inject
} from '@angular/core';

import { CommonModule } from '@angular/common';

import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { TripList } from '../components/trip-list/trip-list';
import {TravelTip} from '../components/travel-tip/travel-tip';
import { HeroHeader, HeroView } from '../components/hero-header/hero-header';
import {ItineraryDetails} from '../components/itinerary-details/itinerary-details';
import { ItineraryTab, ItineraryTabs } from '../components/itinerary-tabs/itinerary-tabs';
import { DayTimeline } from '../components/day-timeline/day-timeline';
import { ShareButton } from '../components/share-button/share-button';
import { ItineraryService } from '../../../core/services/itinerary.service';
import { Trip } from '../../../models/trip';
import { TripDay } from '../../../models/trip-day';
import { Activity } from '../../../models/activity';
import { TravelTip as TravelTipModel } from '../../../models/travel-tip';
import { EMPTY, forkJoin, Observable, of } from 'rxjs';
import { catchError, finalize, switchMap, tap } from 'rxjs/operators';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
@Component({

    selector: 'app-itinerary',

    standalone: true,

    imports: [

        CommonModule,
        MatCardModule,
        MatIconModule,
        TripList,
        TravelTip,
        HeroHeader,
        ItineraryTabs,
        DayTimeline,
        ItineraryDetails,
        ShareButton
    ],

    templateUrl: './itinerary.html',

    styleUrl: './itinerary.scss',

    changeDetection: ChangeDetectionStrategy.OnPush

})

export class Itinerary implements OnInit {

    trips: Trip[] = [];

    tripDays: TripDay[] = [];

    activities: Activity[] = [];

    travelTip: TravelTipModel | null = null;

    selectedTrip: Trip | null = null;

    selectedDay: TripDay | null = null;

    activeView: HeroView = 'My Trips';

    selectedTab: ItineraryTab = 'Itinerary';

    notesDraft = '';

    actionFeedback: string | null = null;

    loading = false;

    error: string | null = null;

    private readonly notesByTripDay: Record<string, string[]> = {};

    private readonly itineraryService = inject(ItineraryService);

    private readonly cdr = inject(ChangeDetectorRef);

    private readonly destroyRef = inject(DestroyRef);

    ngOnInit(): void {

        this.loadInitialData();

    }

    onTripSelected(trip: Trip): void {

        if (this.selectedTrip?.id === trip.id) {
            return;
        }

        this.selectedTrip = trip;
        this.error = null;
        this.loading = true;

        this.itineraryService
            .getTripDays(trip.id)
            .pipe(
                tap(days => {
                    this.tripDays = days;
                    this.selectedDay = days[0] ?? null;
                }),
                switchMap(() => this.loadSelectedDayData()),
                finalize(() => {
                    this.loading = false;
                    this.cdr.markForCheck();
                }),
                catchError(() => {
                    this.setErrorState('Unable to load trip details at the moment.');
                    return EMPTY;
                }),
                takeUntilDestroyed(this.destroyRef)
            )
            .subscribe();

    }

    onDaySelected(day: TripDay): void {

        if (!this.selectedTrip || this.selectedDay?.dayNumber === day.dayNumber) {
            return;
        }

        this.selectedDay = day;
        this.error = null;
        this.loading = true;

        this.loadSelectedDayData()
            .pipe(
                finalize(() => {
                    this.loading = false;
                    this.cdr.markForCheck();
                }),
                catchError(() => {
                    this.setErrorState('Unable to load selected day details.');
                    return EMPTY;
                }),
                takeUntilDestroyed(this.destroyRef)
            )
            .subscribe();

    }

    onViewChanged(view: HeroView): void {

        if (this.activeView === view) {
            return;
        }

        this.activeView = view;
        this.selectedTab = 'Itinerary';
        this.notesDraft = '';
        this.actionFeedback = null;
        this.loadInitialData();

    }

    onTabChanged(tab: ItineraryTab): void {

        this.selectedTab = tab;
        this.actionFeedback = null;

    }

    onNotesInput(event: Event): void {

        const target = event.target as HTMLTextAreaElement;
        this.notesDraft = target.value;

    }

    addNote(): void {

        const value = this.notesDraft.trim();

        if (!value) {
            return;
        }

        const key = this.getTripDayKey();

        if (!key) {
            this.actionFeedback = 'Select a trip day first to add notes.';
            return;
        }

        const existingNotes = this.notesByTripDay[key] ?? [];
        this.notesByTripDay[key] = [...existingNotes, value];
        this.notesDraft = '';
        this.actionFeedback = 'Note added to selected day.';
        this.cdr.markForCheck();

    }

    removeNote(index: number): void {

        const key = this.getTripDayKey();

        if (!key || !this.notesByTripDay[key]) {
            return;
        }

        this.notesByTripDay[key] = this.notesByTripDay[key].filter((_, i) => i !== index);
        this.actionFeedback = 'Note removed.';
        this.cdr.markForCheck();

    }

    async shareItinerary(): Promise<void> {

        if (!this.selectedTrip) {
            this.actionFeedback = 'Select a trip before sharing.';
            this.cdr.markForCheck();
            return;
        }

        const itineraryUrl = this.buildShareUrl();
        const sharePayload = {
            title: `${this.selectedTrip.name} itinerary`,
            text: `Explore my itinerary for ${this.selectedTrip.destinationCity}.`,
            url: itineraryUrl
        };

        try {
            if ('share' in navigator && typeof navigator.share === 'function') {
                await navigator.share(sharePayload);
                this.actionFeedback = 'Itinerary shared successfully.';
            } else if (navigator.clipboard?.writeText) {
                await navigator.clipboard.writeText(itineraryUrl);
                this.actionFeedback = 'Share link copied to clipboard.';
            } else {
                this.actionFeedback = 'Sharing is not supported in this browser.';
            }
        } catch {
            this.actionFeedback = 'Unable to share itinerary right now.';
        }

        this.cdr.markForCheck();

    }

    exportPdf(): void {

        if (!this.selectedTrip) {
            this.actionFeedback = 'Select a trip before exporting.';
            this.cdr.markForCheck();
            return;
        }

        const exportWindow = window.open('', '_blank', 'noopener,noreferrer,width=1200,height=900');

        if (!exportWindow) {
            this.actionFeedback = 'Pop-up blocked. Allow pop-ups to export PDF.';
            this.cdr.markForCheck();
            return;
        }

        exportWindow.document.write(this.buildExportHtml());
        exportWindow.document.close();
        exportWindow.focus();
        exportWindow.print();
        exportWindow.close();

        this.actionFeedback = 'Export opened. Choose Save as PDF in print dialog.';
        this.cdr.markForCheck();

    }

    openMapForSelectedDay(): void {

        const locations = this.mapLocations;

        if (locations.length === 0) {
            this.actionFeedback = 'No locations available for selected day.';
            this.cdr.markForCheck();
            return;
        }

        this.openMap(locations[0]);

    }

    openMap(location: string): void {

        const mapsUrl = `https://www.google.com/maps/search/?api=1&query=${encodeURIComponent(location)}`;
        window.open(mapsUrl, '_blank', 'noopener,noreferrer');

    }

    get totalEstimatedCost(): number {

        return this.activities.reduce((sum, activity) => sum + activity.estimatedCost, 0);

    }

    get confirmedActivitiesCount(): number {

        return this.activities.filter(activity => activity.status === 'Confirmed').length;

    }

    get pendingActivitiesCount(): number {

        return this.activities.filter(activity => activity.status === 'Pending').length;

    }

    get suggestedActivitiesCount(): number {

        return this.activities.filter(activity => activity.status === 'Suggested').length;

    }

    get bookings(): Activity[] {

        return this.activities.filter(activity => activity.category === 'Flight' || activity.category === 'Hotel' || activity.category === 'Transport');

    }

    get selectedDayNotes(): string[] {

        const key = this.getTripDayKey();

        if (!key) {
            return [];
        }

        return this.notesByTripDay[key] ?? [];

    }

    get mapLocations(): string[] {

        return Array.from(new Set(this.activities.map(activity => activity.locationName).filter(Boolean)));

    }

    get activeScopeLabel(): string {

        return this.activeView === 'Shared With Me' ? 'Shared itineraries' : 'Your itineraries';

    }

    private loadInitialData(): void {

        this.loading = true;
        this.error = null;

        this.itineraryService
            .getTrips(this.activeView === 'Shared With Me' ? 'shared' : 'my')
            .pipe(
                tap(trips => {
                    this.trips = trips;
                    this.selectedTrip = trips[0] ?? null;
                }),
                switchMap(() => {
                    if (!this.selectedTrip) {
                        this.tripDays = [];
                        this.selectedDay = null;
                        this.activities = [];
                        this.travelTip = null;
                        this.actionFeedback = null;
                        return of(void 0);
                    }

                    return this.itineraryService.getTripDays(this.selectedTrip.id).pipe(
                        tap(days => {
                            this.tripDays = days;
                            this.selectedDay = days[0] ?? null;
                        }),
                        switchMap(() => this.loadSelectedDayData())
                    );
                }),
                finalize(() => {
                    this.loading = false;
                    this.cdr.markForCheck();
                }),
                catchError(() => {
                    this.setErrorState('Unable to load itinerary data.');
                    return EMPTY;
                }),
                takeUntilDestroyed(this.destroyRef)
            )
            .subscribe();

    }

    // Centralized day-level loading keeps API replacement isolated to service methods.
    private loadSelectedDayData(): Observable<void> {

        if (!this.selectedTrip || !this.selectedDay) {
            this.activities = [];
            this.travelTip = null;
            return of(void 0);
        }

        return forkJoin({
            activities: this.itineraryService.getActivities(this.selectedTrip.id, this.selectedDay.dayNumber),
            travelTip: this.itineraryService.getTravelTip(this.selectedTrip.id, this.selectedDay.dayNumber)
        }).pipe(
            tap(({ activities, travelTip }) => {
                this.activities = activities;
                this.travelTip = travelTip;
            }),
            switchMap(() => of(void 0))
        );

    }

    private setErrorState(message: string): void {

        this.error = message;
        this.activities = [];
        this.travelTip = null;
        this.loading = false;
        this.cdr.markForCheck();

    }

    private getTripDayKey(): string | null {

        if (!this.selectedTrip || !this.selectedDay) {
            return null;
        }

        return `${this.selectedTrip.id}-${this.selectedDay.dayNumber}`;

    }

    private buildShareUrl(): string {

        const tripId = this.selectedTrip?.id ?? '';
        const dayNumber = this.selectedDay?.dayNumber ?? 1;

        return `${window.location.origin}/itinerary?tripId=${tripId}&day=${dayNumber}`;

    }

    private buildExportHtml(): string {

        const tripTitle = this.escapeHtml(this.selectedTrip?.name ?? 'Itinerary');
        const destination = this.escapeHtml(this.selectedTrip ? `${this.selectedTrip.destinationCity}, ${this.selectedTrip.destinationCountry}` : '');
        const dayLabel = this.escapeHtml(this.selectedDay?.title ?? 'Selected Day');

        const activityItems = this.activities
            .map(activity => `<li><strong>${this.escapeHtml(activity.startTime)}-${this.escapeHtml(activity.endTime)}</strong> ${this.escapeHtml(activity.title)} (${this.escapeHtml(activity.locationName)})</li>`)
            .join('');

        const noteItems = this.selectedDayNotes
            .map(note => `<li>${this.escapeHtml(note)}</li>`)
            .join('');

        return `<!DOCTYPE html>
<html>
<head>
  <meta charset="UTF-8" />
  <title>${tripTitle} - Export</title>
  <style>
    body { font-family: Arial, sans-serif; color: #0f172a; padding: 32px; }
    h1 { margin: 0 0 8px; }
    h2 { margin: 24px 0 8px; }
    .meta { color: #475569; margin-bottom: 16px; }
    ul { margin: 8px 0 0; padding-left: 20px; }
    .chip { display: inline-block; padding: 4px 10px; border-radius: 999px; background: #e2e8f0; margin-right: 6px; }
  </style>
</head>
<body>
  <h1>${tripTitle}</h1>
  <div class="meta">${destination} | ${dayLabel}</div>
  <div>
    <span class="chip">Confirmed: ${this.confirmedActivitiesCount}</span>
    <span class="chip">Pending: ${this.pendingActivitiesCount}</span>
    <span class="chip">Suggested: ${this.suggestedActivitiesCount}</span>
    <span class="chip">Est. Cost: ${this.selectedTrip?.budgetCurrency ?? 'USD'} ${this.totalEstimatedCost}</span>
  </div>
  <h2>Activities</h2>
  <ul>${activityItems || '<li>No activities available.</li>'}</ul>
  <h2>Notes</h2>
  <ul>${noteItems || '<li>No notes added.</li>'}</ul>
</body>
</html>`;

    }

    private escapeHtml(value: string): string {

        return value
            .replaceAll('&', '&amp;')
            .replaceAll('<', '&lt;')
            .replaceAll('>', '&gt;')
            .replaceAll('"', '&quot;')
            .replaceAll('\'', '&#039;');

    }

}