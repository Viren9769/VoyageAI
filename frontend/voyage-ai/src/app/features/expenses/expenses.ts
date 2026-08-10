import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';

import { ExpenseService } from '../../core/services/expense.service';
import {
  AddExpensePayload,
  BudgetComparisonItem,
  CategorySpend,
  ExpenseItem,
  ExpenseSummary,
  TripExpenseContext,
} from '../../models/expense';
import { AddExpense } from './components/add-expense/add-expense';
import { BudgetComparison } from './components/budget-comparison/budget-comparison';
import { ExpenseHeader } from './components/expense-header/expense-header';
import { ExpenseList } from './components/expense-list/expense-list';
import { ExpenseSummary as ExpenseSummaryComponent } from './components/expense-summary/expense-summary';
import { SpendingCategory } from './components/spending-category/spending-category';
import { TripSelector } from './components/trip-selector/trip-selector';

@Component({
  selector: 'app-expenses',
  standalone: true,
  imports: [
    CommonModule,
    ExpenseHeader,
    ExpenseSummaryComponent,
    SpendingCategory,
    BudgetComparison,
    ExpenseList,
    AddExpense,
    TripSelector,
  ],
  templateUrl: './expenses.html',
  styleUrl: './expenses.scss',
})
export class Expenses implements OnInit {
  trips: TripExpenseContext[] = [];
  selectedTripId = '';

  summary: ExpenseSummary = {
    totalBudget: 0,
    totalSpent: 0,
    remaining: 0,
    dailyAverage: 0,
  };

  categories: CategorySpend[] = [];
  budgetComparison: BudgetComparisonItem[] = [];
  expenses: ExpenseItem[] = [];

  isAddExpenseOpen = false;

  constructor(private readonly expenseService: ExpenseService) {}

  ngOnInit(): void {
    this.expenseService.getTrips().subscribe((trips) => {
      this.trips = trips;
      this.selectedTripId = trips[0]?.id ?? '';
      this.loadDashboard();
    });
  }

  onTripChange(nextTripId: string): void {
    this.selectedTripId = nextTripId;
    this.loadDashboard();
  }

  openAddExpense(): void {
    this.isAddExpenseOpen = true;
  }

  closeAddExpense(): void {
    this.isAddExpenseOpen = false;
  }

  onAddExpense(payload: AddExpensePayload): void {
    this.expenseService.addExpense(this.selectedTripId, payload).subscribe(() => {
      this.loadDashboard();
      this.closeAddExpense();
    });
  }

  exportCsv(): void {
    if (!this.expenses.length) {
      return;
    }

    const headers = ['Date', 'Description', 'Note', 'Category', 'Payment Method', 'Amount'];
    const lines = this.expenses.map((expense) =>
      [
        expense.date,
        this.escapeCsv(expense.description),
        this.escapeCsv(expense.note),
        expense.category,
        expense.paymentMethod,
        expense.amount.toFixed(2),
      ].join(',')
    );

    const csv = `${headers.join(',')}\n${lines.join('\n')}`;
    const blob = new Blob([csv], { type: 'text/csv;charset=utf-8;' });
    const url = URL.createObjectURL(blob);

    const anchor = document.createElement('a');
    anchor.href = url;
    anchor.download = 'trip-expenses.csv';
    anchor.click();

    URL.revokeObjectURL(url);
  }

  exportPdf(): void {
    window.print();
  }

  private loadDashboard(): void {
    if (!this.selectedTripId) {
      return;
    }

    this.expenseService.getDashboardData(this.selectedTripId).subscribe((data) => {
      this.summary = data.summary;
      this.categories = data.categories;
      this.budgetComparison = data.budgetComparison;
      this.expenses = data.expenses;
    });
  }

  private escapeCsv(value: string): string {
    return `"${value.replace(/"/g, '""')}"`;
  }
}
