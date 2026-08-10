import { Component, EventEmitter, Input, Output } from '@angular/core';

import { CommonModule } from '@angular/common';

export type ItineraryTab = 'Overview' | 'Itinerary' | 'Bookings' | 'Notes' | 'Map';

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

    @Input()
    selectedTab: ItineraryTab = 'Itinerary';

    @Output()
    tabChanged = new EventEmitter<ItineraryTab>();

    readonly tabs: ItineraryTab[] = [

        'Overview',

        'Itinerary',

        'Bookings',

        'Notes',

        'Map'

    ];

    selectTab(tab: ItineraryTab): void {

        if (this.selectedTab === tab) {
            return;
        }

        this.selectedTab = tab;
        this.tabChanged.emit(tab);

    }

}