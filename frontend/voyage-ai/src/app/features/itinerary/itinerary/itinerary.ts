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
import { HeroHeader } from '../components/hero-header/hero-header';
import {ItineraryDetails} from '../components/itinerary-details/itinerary-details';
import { ItineraryTabs } from '../components/itinerary-tabs/itinerary-tabs';
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

    loading = false;

    error: string | null = null;

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

    private loadInitialData(): void {

        this.loading = true;
        this.error = null;

        this.itineraryService
            .getTrips()
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

}