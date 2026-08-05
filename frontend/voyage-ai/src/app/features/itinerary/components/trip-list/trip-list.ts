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
    selectedTripId: number | null = null;

    @Output()
    tripSelected = new EventEmitter<Trip>();

    selectTrip(trip: Trip): void {

        this.tripSelected.emit(trip);

    }

}