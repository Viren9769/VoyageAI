import { CommonModule } from '@angular/common';
import { Component, Input } from '@angular/core';

import { BudgetComparisonItem } from '../../../../models/expense';

@Component({
  selector: 'app-budget-comparison',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './budget-comparison.html',
  styleUrl: './budget-comparison.scss',
})
export class BudgetComparison {
  @Input() items: BudgetComparisonItem[] = [];

  spentPercent(item: BudgetComparisonItem): number {
    if (!item.budget) {
      return 0;
    }

    return Math.min(100, (item.spent / item.budget) * 100);
  }

  remaining(item: BudgetComparisonItem): number {
    return Math.max(item.budget - item.spent, 0);
  }
}
