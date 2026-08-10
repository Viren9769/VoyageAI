import { CommonModule } from '@angular/common';
import { Component, Input } from '@angular/core';

import { CategorySpend } from '../../../../models/expense';

@Component({
  selector: 'app-spending-category',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './spending-category.html',
  styleUrl: './spending-category.scss',
})
export class SpendingCategory {
  @Input() categories: CategorySpend[] = [];
  @Input() totalSpent = 0;

  get chartGradient(): string {
    if (!this.categories.length || !this.totalSpent) {
      return 'conic-gradient(#334155 0 100%)';
    }

    let currentAngle = 0;
    const segments = this.categories
      .map((category) => {
        const ratio = category.amount / this.totalSpent;
        const span = ratio * 360;
        const start = currentAngle;
        const end = currentAngle + span;
        currentAngle = end;
        return `${category.color} ${start}deg ${end}deg`;
      })
      .join(', ');

    return `conic-gradient(${segments})`;
  }

  categoryPercent(amount: number): number {
    if (!this.totalSpent) {
      return 0;
    }

    return (amount / this.totalSpent) * 100;
  }
}
