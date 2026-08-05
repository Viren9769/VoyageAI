import { ChangeDetectionStrategy, Component, Input } from '@angular/core';

import { CommonModule } from '@angular/common';

import { MatCardModule } from '@angular/material/card';

import { ActivityCard } from '../activity-card/activity-card';
import { Activity } from '../../../../models/activity';

@Component({

    selector: 'app-itinerary-details',

    standalone: true,

    imports: [

        CommonModule,

        MatCardModule,

        ActivityCard

    ],

    templateUrl: './itinerary-details.html',

    styleUrl: './itinerary-details.scss',

    changeDetection: ChangeDetectionStrategy.OnPush

})

export class ItineraryDetails {

    @Input()
    activities: Activity[] = [];

}