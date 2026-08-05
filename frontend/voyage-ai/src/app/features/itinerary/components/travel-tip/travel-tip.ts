import { ChangeDetectionStrategy, Component, Input } from '@angular/core';

import { CommonModule } from '@angular/common';

import { MatCardModule } from '@angular/material/card';

import { MatIconModule } from '@angular/material/icon';
import { TravelTip as TravelTipModel } from '../../../../models/travel-tip';

@Component({

    selector: 'app-travel-tip',

    standalone: true,

    imports: [

        CommonModule,

        MatCardModule,

        MatIconModule

    ],

    templateUrl: './travel-tip.html',

    styleUrl: './travel-tip.scss',

    changeDetection: ChangeDetectionStrategy.OnPush

})

export class TravelTip {

    @Input({ required: true })
    tip!: TravelTipModel;

}