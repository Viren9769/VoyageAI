import { Component, OnInit } from '@angular/core';

import { CommonModule } from '@angular/common';

import { DashboardService } from '../../../core/services/dashboard.service';

import { DashboardData } from '../../../models/dashboard';

import { WelcomeCard } from '../components/welcome-card/welcome-card';

import { StatsCards } from '../components/stats-cards/stats-cards';
import { AiPlannerCard } from '../components/ai-planner-card/ai-planner-card';
import { UpcomingTrip } from '../components/upcoming-trip/upcoming-trip';
import { WeatherCard } from '../components/weather-card/weather-card';
import { Reminders } from '../components/reminders/reminders';
import { TravelMapComponent } from '../components/travel-map/travel-map';
import { ExpenseOverviewComponent } from '../components/expense-overview/expense-overview';
import { RecentTrips } from '../components/recent-trips/recent-trips';
import { TravelerPrfile } from '../components/traveler-prfile/traveler-prfile';

@Component({
  selector: 'app-dashboard',

  standalone: true,

  imports: [
    CommonModule,
    WelcomeCard,
    StatsCards,
    AiPlannerCard,
    UpcomingTrip,
    WeatherCard,
    Reminders,
    TravelMapComponent,
    ExpenseOverviewComponent,
    RecentTrips,
    TravelerPrfile
  ],

  templateUrl: './dashboard.html',

  styleUrl: './dashboard.scss'
})
export class Dashboard implements OnInit {

  dashboard!: DashboardData;

  isLoading = true;

  constructor(
    private dashboardService: DashboardService
  ) {}

  ngOnInit(): void {

    this.loadDashboard();

  }

  private loadDashboard(): void {

    this.dashboardService
      .getDashboard()
      .subscribe({

        next: (response) => {

          this.dashboard = response;

          this.isLoading = false;

        },

        error: (error) => {

          console.error(error);

          this.isLoading = false;

        }

      });

  }

}