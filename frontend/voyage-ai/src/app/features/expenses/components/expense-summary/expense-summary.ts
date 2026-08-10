import { CommonModule } from '@angular/common';
import { Component, Input } from '@angular/core';
import { MatIconModule } from '@angular/material/icon';

import { ExpenseSummary as ExpenseSummaryModel } from '../../../../models/expense';

@Component({
  selector: 'app-expense-summary',
  standalone: true,
  imports: [CommonModule, MatIconModule],
  templateUrl: './expense-summary.html',
  styleUrl: './expense-summary.scss',
})
export class ExpenseSummary {
  @Input() summary: ExpenseSummaryModel = {
    totalBudget: 0,
    totalSpent: 0,
    remaining: 0,
    dailyAverage: 0,
  };

  get spentPercent(): number {
    if (!this.summary.totalBudget) {
      return 0;
    }

    return Math.min(100, (this.summary.totalSpent / this.summary.totalBudget) * 100);
  }

  get remainingPercent(): number {
    if (!this.summary.totalBudget) {
      return 0;
    }

    return Math.max(0, (this.summary.remaining / this.summary.totalBudget) * 100);
  }
}
