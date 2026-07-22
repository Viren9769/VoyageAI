import { Component } from '@angular/core';

import { CommonModule } from '@angular/common';

import { FormsModule } from '@angular/forms';

import { MatButtonModule } from '@angular/material/button';

import { MatSelectModule } from '@angular/material/select';

import { TripService, TripFilter } from '../../../../../core/services/trip.service';

@Component({
  selector: 'app-trip-filters',

  standalone: true,

  imports: [
    CommonModule,
    FormsModule,
    MatButtonModule,
    MatSelectModule
  ],

  templateUrl: './trip-filters.html',

  styleUrl: './trip-filters.scss'
})

export class TripFilters {

  constructor(
    private tripService: TripService
  ) {}

  selectedTab: TripFilter = 'All Trips';

  sortBy = 'Recently Updated';

  tabs: TripFilter[] = [
    'All Trips',
    'Upcoming',
    'Ongoing',
    'Completed',
    'Drafts'
  ];

  sortOptions = [
    'Recently Updated',
    'Newest',
    'Oldest'
  ];

  /**
   * Tab Click
   */
  selectTab(tab: TripFilter): void {

    this.selectedTab = tab;

    this.tripService.setFilter(tab);

  }

  /**
   * Sort Changed
   */
  changeSort(sort: string): void {

    this.sortBy = sort;

    this.tripService.setSort(sort);

  }

}