import { Component } from '@angular/core';

import { CommonModule } from '@angular/common';

@Component({

    selector: 'app-itinerary-tabs',

    standalone: true,

    imports: [

        CommonModule

    ],

    templateUrl: './itinerary-tabs.html',

    styleUrl: './itinerary-tabs.scss'

})

export class ItineraryTabs {

    selectedTab = 'Itinerary';

    tabs = [

        'Overview',

        'Itinerary',

        'Bookings',

        'Notes',

        'Map'

    ];

    selectTab(tab: string): void {

        this.selectedTab = tab;

    }

}