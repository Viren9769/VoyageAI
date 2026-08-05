import { ChangeDetectionStrategy, Component, EventEmitter, Input, Output } from '@angular/core';

import { CommonModule } from '@angular/common';

import { TripDay } from '../../../../models/trip-day';

@Component({

    selector: 'app-day-timeline',

    standalone: true,

    imports: [

        CommonModule

    ],

    templateUrl: './day-timeline.html',

    styleUrl: './day-timeline.scss',

    changeDetection: ChangeDetectionStrategy.OnPush

})

export class DayTimeline {

    @Input()
    days: TripDay[] = [];

    @Input()
    selectedDayNumber: number | null = null;

    @Output()
    daySelected = new EventEmitter<TripDay>();

    selectDay(day: TripDay): void {

        this.daySelected.emit(day);

    }

}