import { CommonModule } from '@angular/common';
import { Component, Input, OnChanges, SimpleChanges } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MatIconModule } from '@angular/material/icon';

import { ExpenseCategory, ExpenseItem, PaymentMethod } from '../../../../models/expense';

@Component({
  selector: 'app-expense-list',
  standalone: true,
  imports: [CommonModule, FormsModule, MatIconModule],
  templateUrl: './expense-list.html',
  styleUrl: './expense-list.scss',
})
export class ExpenseList implements OnChanges {
  @Input() expenses: ExpenseItem[] = [];

  searchTerm = '';
  selectedCategory: ExpenseCategory | 'All' = 'All';
  selectedPayment: PaymentMethod | 'All' = 'All';
  selectedDateRange = 'all';

  currentPage = 1;
  pageSize = 5;

  readonly pageSizeOptions = [5, 10, 15];

  filteredExpenses: ExpenseItem[] = [];
  paginatedExpenses: ExpenseItem[] = [];

  get categoryOptions(): Array<ExpenseCategory | 'All'> {
    const categories = Array.from(new Set(this.expenses.map((expense) => expense.category)));
    return ['All', ...categories];
  }

  get paymentOptions(): Array<PaymentMethod | 'All'> {
    const methods = Array.from(new Set(this.expenses.map((expense) => expense.paymentMethod)));
    return ['All', ...methods];
  }

  get totalPages(): number {
    return Math.max(1, Math.ceil(this.filteredExpenses.length / this.pageSize));
  }

  get visiblePages(): number[] {
    const maxPagesToShow = 5;

    if (this.totalPages <= maxPagesToShow) {
      return Array.from({ length: this.totalPages }, (_, index) => index + 1);
    }

    const half = Math.floor(maxPagesToShow / 2);
    let start = Math.max(1, this.currentPage - half);
    let end = Math.min(this.totalPages, start + maxPagesToShow - 1);

    if (end - start + 1 < maxPagesToShow) {
      start = Math.max(1, end - maxPagesToShow + 1);
    }

    return Array.from({ length: end - start + 1 }, (_, index) => start + index);
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['expenses']) {
      this.applyFilters(true);
    }
  }

  applyFilters(resetPage = false): void {
    const normalizedSearch = this.searchTerm.trim().toLowerCase();

    this.filteredExpenses = this.expenses.filter((expense) => {
      const isMatchSearch =
        !normalizedSearch ||
        expense.description.toLowerCase().includes(normalizedSearch) ||
        expense.note.toLowerCase().includes(normalizedSearch);

      const isMatchCategory =
        this.selectedCategory === 'All' || expense.category === this.selectedCategory;

      const isMatchPayment =
        this.selectedPayment === 'All' || expense.paymentMethod === this.selectedPayment;

      const isMatchDate = this.isWithinDateRange(expense.date);

      return isMatchSearch && isMatchCategory && isMatchPayment && isMatchDate;
    });

    if (resetPage) {
      this.currentPage = 1;
    }

    if (this.currentPage > this.totalPages) {
      this.currentPage = this.totalPages;
    }

    this.setCurrentPageSlice();
  }

  nextPage(): void {
    if (this.currentPage < this.totalPages) {
      this.currentPage += 1;
      this.setCurrentPageSlice();
    }
  }

  previousPage(): void {
    if (this.currentPage > 1) {
      this.currentPage -= 1;
      this.setCurrentPageSlice();
    }
  }

  goToPage(page: number): void {
    if (page < 1 || page > this.totalPages) {
      return;
    }

    this.currentPage = page;
    this.setCurrentPageSlice();
  }

  changePageSize(size: number): void {
    this.pageSize = Number(size);
    this.currentPage = 1;
    this.setCurrentPageSlice();
  }

  onFilterInput(): void {
    this.applyFilters(true);
  }

  categoryClass(category: ExpenseCategory): string {
    return category
      .toLowerCase()
      .replace('&', 'and')
      .replace(/\s+/g, '-');
  }

  private setCurrentPageSlice(): void {
    const start = (this.currentPage - 1) * this.pageSize;
    this.paginatedExpenses = this.filteredExpenses.slice(start, start + this.pageSize);
  }

  private isWithinDateRange(dateValue: string): boolean {
    if (this.selectedDateRange === 'all') {
      return true;
    }

    const date = new Date(dateValue);

    if (this.selectedDateRange === 'last7') {
      const days = this.daysFromToday(date);
      return days >= 0 && days <= 7;
    }

    if (this.selectedDateRange === 'last30') {
      const days = this.daysFromToday(date);
      return days >= 0 && days <= 30;
    }

    return true;
  }

  private daysFromToday(date: Date): number {
    const now = new Date();
    const diffMs = now.getTime() - date.getTime();
    return Math.floor(diffMs / (1000 * 60 * 60 * 24));
  }
}
