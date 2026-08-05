import { Component, Input } from '@angular/core';

import { CommonModule } from '@angular/common';

@Component({

    selector: 'app-trip-card',

    standalone: true,

    imports: [

        CommonModule

    ],

    templateUrl: './trip-card.html',

    styleUrl: './trip-card.scss'

})

export class TripCard {

    @Input({ required: true })

    trip!: {

        name: string;

        image: string;

        date: string;

        duration: string;

        status: string;

    };

}