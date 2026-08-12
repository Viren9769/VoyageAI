import { CommonModule } from '@angular/common';
import { Component, Input } from '@angular/core';
import { MatIconModule } from '@angular/material/icon';

import { TravelerStats as TravelerStatsModel } from '../../../../models/traveler';

@Component({
  selector: 'app-traveler-stats',
  standalone: true,
  imports: [CommonModule, MatIconModule],
  templateUrl: './traveler-stats.html',
  styleUrl: './traveler-stats.scss',
})
export class TravelerStats {
  @Input() stats!: TravelerStatsModel;
}
