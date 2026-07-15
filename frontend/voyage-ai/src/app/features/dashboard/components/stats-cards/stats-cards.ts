import { Component, Input } from '@angular/core';

import { CommonModule } from '@angular/common';

import { MatIconModule } from '@angular/material/icon';

import { DashboardStat } from '../../../../models/dashboard';

@Component({
  selector: 'app-stats-cards',
  standalone: true,
  imports: [
    CommonModule,
    MatIconModule
  ],
  templateUrl: './stats-cards.html',
  styleUrl: './stats-cards.scss'
})
export class StatsCards {

  @Input()
  stats: DashboardStat[] = [];

}