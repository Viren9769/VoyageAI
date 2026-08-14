import { ChangeDetectionStrategy, Component, EventEmitter, Input, Output } from '@angular/core';

import { CommonModule } from '@angular/common';

import { MatCardModule } from '@angular/material/card';

import { MatFormFieldModule } from '@angular/material/form-field';

import { MatInputModule } from '@angular/material/input';

import { MatSelectModule } from '@angular/material/select';

import { MatIconModule } from '@angular/material/icon';

import { Trip } from '../../../../models/trip';

@Component({

    selector: 'app-trip-list',

    standalone: true,

    imports: [

        CommonModule,

        MatCardModule,

        MatFormFieldModule,

        MatInputModule,

        MatSelectModule,

        MatIconModule

    ],

    templateUrl: './trip-list.html',

    styleUrl: './trip-list.scss',

    changeDetection: ChangeDetectionStrategy.OnPush

})

export class TripList {

    @Input()
    trips: Trip[] = [];

    @Input()
    selectedTripId: string | number | null = null;

    get selectedTripIdText(): string | null {
        if (this.selectedTripId === null || this.selectedTripId === undefined) {
            return null;
        }

        return String(this.selectedTripId);
    }

    @Output()
    tripSelected = new EventEmitter<Trip>();

    searchQuery = '';

    selectedStatus: 'All' | Trip['status'] = 'All';

    get statusOptions(): Array<'All' | Trip['status']> {

        const statuses = new Set<Trip['status']>();

        for (const trip of this.trips) {
            statuses.add(trip.status);
        }

        return ['All', ...Array.from(statuses)];

    }

    get filteredTrips(): Trip[] {

        const normalizedQuery = this.searchQuery.trim().toLowerCase();

        return this.trips.filter(trip => {

            const matchesStatus = this.selectedStatus === 'All' || trip.status === this.selectedStatus;

            if (!matchesStatus) {
                return false;
            }

            if (!normalizedQuery) {
                return true;
            }

            const searchableText = [
                trip.name,
                trip.destinationCity,
                trip.destinationCountry,
                trip.status
            ].join(' ').toLowerCase();

            return searchableText.includes(normalizedQuery);
        });

    }

    onSearchInput(event: Event): void {

        const target = event.target as HTMLInputElement;
        this.searchQuery = target.value;

    }

    onStatusChange(value: string | null): void {

        if (!value) {
            this.selectedStatus = 'All';
            return;
        }

        this.selectedStatus = value as 'All' | Trip['status'];

    }

    selectTrip(trip: Trip): void {

        this.tripSelected.emit(trip);

    }

}