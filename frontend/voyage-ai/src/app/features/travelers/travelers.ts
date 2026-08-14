import { CommonModule } from '@angular/common';
import { Component, OnInit, inject } from '@angular/core';

import { TravelerService, TripOption } from '../../core/services/traveler.service';
import { Traveler, TravelerFormPayload, TravelerStats } from '../../models/traveler';
import { TravelerHeader } from './components/traveler-header/traveler-header';
import { TravelerStats as TravelerStatsComponent } from './components/traveler-stats/traveler-stats';
import { TravelerList } from './components/traveler-list/traveler-list';
import { TravelerForm } from './components/traveler-form/traveler-form';
import { TravelerDetail } from './components/traveler-detail/traveler-detail';

@Component({
  selector: 'app-travelers',
  standalone: true,
  imports: [CommonModule, TravelerHeader, TravelerStatsComponent, TravelerList, TravelerForm, TravelerDetail],
  templateUrl: './travelers.html',
  styleUrl: './travelers.scss',
})
export class Travelers implements OnInit {
  travelers: Traveler[] = [];
  stats: TravelerStats = { totalTravelers: 0, adults: 0, children: 0, upcomingTrips: 0 };
  tripOptions: TripOption[] = [];

  isFormOpen = false;
  isDetailOpen = false;
  editTarget: Traveler | null = null;
  detailTarget: Traveler | null = null;

  private readonly travelerService = inject(TravelerService);

  ngOnInit(): void {
    this.travelerService.loadData().subscribe((result: { travelers: Traveler[]; stats: TravelerStats; tripOptions: TripOption[] }) => {
      const { travelers, stats, tripOptions } = result;
      this.travelers = travelers;
      this.stats = stats;
      this.tripOptions = tripOptions;
    });
  }

  onAddTraveler(): void {
    this.editTarget = null;
    this.isFormOpen = true;
  }

  onView(traveler: Traveler): void {
    this.detailTarget = traveler;
    this.isDetailOpen = true;
  }

  onEdit(traveler: Traveler): void {
    this.isDetailOpen = false;
    this.editTarget = traveler;
    this.isFormOpen = true;
  }

  onDelete(traveler: Traveler): void {
    this.travelerService.deleteTraveler(traveler.id).subscribe(() => this.reload());
  }

  onFormSave(payload: TravelerFormPayload): void {
    const op$ = this.editTarget
      ? this.travelerService.editTraveler(this.editTarget.id, payload)
      : this.travelerService.addTraveler(payload);

    op$.subscribe(() => {
      this.isFormOpen = false;
      this.editTarget = null;
      this.reload();
    });
  }

  onFormClose(): void {
    this.isFormOpen = false;
    this.editTarget = null;
  }

  onDetailClose(): void {
    this.isDetailOpen = false;
    this.detailTarget = null;
  }

  private reload(): void {
    this.travelerService.loadData().subscribe((result: { travelers: Traveler[]; stats: TravelerStats; tripOptions: TripOption[] }) => {
      const { travelers, stats, tripOptions } = result;
      this.travelers = travelers;
      this.stats = stats;
      this.tripOptions = tripOptions;
    });
  }
}
