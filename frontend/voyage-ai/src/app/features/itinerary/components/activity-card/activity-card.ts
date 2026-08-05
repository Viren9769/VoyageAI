import { Component, Input } from '@angular/core';

import { CommonModule } from '@angular/common';

import { MatIconModule } from '@angular/material/icon';
import { Activity } from '../../../../models/activity';

@Component({

    selector: 'app-activity-card',

    standalone: true,

    imports: [

        CommonModule,

        MatIconModule

    ],

    templateUrl: './activity-card.html',

    styleUrl: './activity-card.scss'

})

export class ActivityCard {

    @Input({ required: true })

    activity!: Activity;

}