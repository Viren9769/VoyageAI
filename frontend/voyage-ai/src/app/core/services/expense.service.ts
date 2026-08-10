import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';

import {
  AddExpensePayload,
  BudgetComparisonItem,
  CategorySpend,
  ExpenseDashboardData,
  ExpenseItem,
  ExpenseSummary,
  TripExpenseContext,
} from '../../models/expense';

interface TripExpenseStore {
  trip: TripExpenseContext;
  summary: ExpenseSummary;
  categories: CategorySpend[];
  budgetComparison: BudgetComparisonItem[];
  expenses: ExpenseItem[];
}

@Injectable({
  providedIn: 'root',
})
export class ExpenseService {
  private readonly tripStores: TripExpenseStore[] = [
    {
      trip: {
        id: 'trip-1',
        title: 'Switzerland Escape',
        startDate: '2026-06-17',
        endDate: '2026-06-25',
        days: 8,
        status: 'Active',
        budget: 4500,
        coverImage: 'images/switzerland.jpg',
      },
      summary: {
        totalBudget: 4500,
        totalSpent: 2780,
        remaining: 1720,
        dailyAverage: 347.5,
      },
      categories: [
        { category: 'Accommodation', amount: 975, color: '#6d5efc' },
        { category: 'Transportation', amount: 695, color: '#4ea3ff' },
        { category: 'Food & Dining', amount: 605, color: '#58d17a' },
        { category: 'Activities', amount: 335, color: '#f8a333' },
        { category: 'Shopping', amount: 115, color: '#f56585' },
        { category: 'Others', amount: 55, color: '#b7c2d9' },
      ],
      budgetComparison: [
        { category: 'Accommodation', budget: 1200, spent: 975 },
        { category: 'Transportation', budget: 1000, spent: 695 },
        { category: 'Food & Dining', budget: 700, spent: 605 },
        { category: 'Activities', budget: 600, spent: 335 },
        { category: 'Shopping', budget: 300, spent: 115 },
        { category: 'Others', budget: 200, spent: 55 },
      ],
      expenses: [
        {
          id: 'exp-1',
          date: '2026-06-18',
          description: 'Flight to Zurich',
          note: 'Houston -> Zurich',
          category: 'Transportation',
          paymentMethod: 'Credit Card',
          amount: 450,
        },
        {
          id: 'exp-2',
          date: '2026-06-18',
          description: 'Hilton Zurich Airport',
          note: '1 Night Stay',
          category: 'Accommodation',
          paymentMethod: 'Credit Card',
          amount: 320,
        },
        {
          id: 'exp-3',
          date: '2026-06-18',
          description: 'Dinner at Swiss Restaurant',
          note: 'Traditional Swiss Cuisine',
          category: 'Food & Dining',
          paymentMethod: 'Cash',
          amount: 65,
        },
        {
          id: 'exp-4',
          date: '2026-06-19',
          description: 'Swiss National Museum',
          note: 'Entry Ticket',
          category: 'Activities',
          paymentMethod: 'Credit Card',
          amount: 42,
        },
        {
          id: 'exp-5',
          date: '2026-06-19',
          description: 'Local Train Pass',
          note: 'Zurich Day Pass',
          category: 'Transportation',
          paymentMethod: 'Credit Card',
          amount: 28,
        },
        {
          id: 'exp-6',
          date: '2026-06-20',
          description: 'Souvenirs at Bahnhofstrasse',
          note: 'Gifts and chocolates',
          category: 'Shopping',
          paymentMethod: 'Wallet',
          amount: 115,
        },
      ],
    },
    {
      trip: {
        id: 'trip-2',
        title: 'Bali Adventure',
        startDate: '2026-04-04',
        endDate: '2026-04-11',
        days: 8,
        status: 'Completed',
        budget: 3200,
        coverImage: 'images/bali.jpeg',
      },
      summary: {
        totalBudget: 3200,
        totalSpent: 1840,
        remaining: 1360,
        dailyAverage: 230,
      },
      categories: [
        { category: 'Accommodation', amount: 760, color: '#6d5efc' },
        { category: 'Transportation', amount: 420, color: '#4ea3ff' },
        { category: 'Food & Dining', amount: 360, color: '#58d17a' },
        { category: 'Activities', amount: 190, color: '#f8a333' },
        { category: 'Shopping', amount: 80, color: '#f56585' },
        { category: 'Others', amount: 30, color: '#b7c2d9' },
      ],
      budgetComparison: [
        { category: 'Accommodation', budget: 950, spent: 760 },
        { category: 'Transportation', budget: 650, spent: 420 },
        { category: 'Food & Dining', budget: 500, spent: 360 },
        { category: 'Activities', budget: 400, spent: 190 },
        { category: 'Shopping', budget: 350, spent: 80 },
        { category: 'Others', budget: 350, spent: 30 },
      ],
      expenses: [
        {
          id: 'bali-1',
          date: '2026-04-05',
          description: 'Airport Transfer',
          note: 'Ngurah Rai to Ubud',
          category: 'Transportation',
          paymentMethod: 'Credit Card',
          amount: 48,
        },
        {
          id: 'bali-2',
          date: '2026-04-06',
          description: 'Villa Stay',
          note: 'Ubud private villa',
          category: 'Accommodation',
          paymentMethod: 'Bank Transfer',
          amount: 260,
        },
      ],
    },
    {
      trip: {
        id: 'trip-3',
        title: 'Iceland Road Trip',
        startDate: '2026-09-09',
        endDate: '2026-09-16',
        days: 8,
        status: 'Draft',
        budget: 2800,
        coverImage: 'images/iceland.jpeg',
      },
      summary: {
        totalBudget: 2800,
        totalSpent: 860,
        remaining: 1940,
        dailyAverage: 107.5,
      },
      categories: [
        { category: 'Accommodation', amount: 300, color: '#6d5efc' },
        { category: 'Transportation', amount: 360, color: '#4ea3ff' },
        { category: 'Food & Dining', amount: 120, color: '#58d17a' },
        { category: 'Activities', amount: 60, color: '#f8a333' },
        { category: 'Shopping', amount: 20, color: '#f56585' },
        { category: 'Others', amount: 0, color: '#b7c2d9' },
      ],
      budgetComparison: [
        { category: 'Accommodation', budget: 700, spent: 300 },
        { category: 'Transportation', budget: 900, spent: 360 },
        { category: 'Food & Dining', budget: 450, spent: 120 },
        { category: 'Activities', budget: 350, spent: 60 },
        { category: 'Shopping', budget: 200, spent: 20 },
        { category: 'Others', budget: 200, spent: 0 },
      ],
      expenses: [
        {
          id: 'ice-1',
          date: '2026-07-01',
          description: '4x4 rental deposit',
          note: 'Self-drive ring road',
          category: 'Transportation',
          paymentMethod: 'Credit Card',
          amount: 360,
        },
      ],
    },
    {
      trip: {
        id: 'trip-4',
        title: 'New York City Break',
        startDate: '2026-11-17',
        endDate: '2026-11-22',
        days: 6,
        status: 'Draft',
        budget: 2100,
        coverImage: 'images/paris.jpeg',
      },
      summary: {
        totalBudget: 2100,
        totalSpent: 420,
        remaining: 1680,
        dailyAverage: 70,
      },
      categories: [
        { category: 'Accommodation', amount: 150, color: '#6d5efc' },
        { category: 'Transportation', amount: 90, color: '#4ea3ff' },
        { category: 'Food & Dining', amount: 90, color: '#58d17a' },
        { category: 'Activities', amount: 70, color: '#f8a333' },
        { category: 'Shopping', amount: 20, color: '#f56585' },
        { category: 'Others', amount: 0, color: '#b7c2d9' },
      ],
      budgetComparison: [
        { category: 'Accommodation', budget: 600, spent: 150 },
        { category: 'Transportation', budget: 450, spent: 90 },
        { category: 'Food & Dining', budget: 450, spent: 90 },
        { category: 'Activities', budget: 300, spent: 70 },
        { category: 'Shopping', budget: 200, spent: 20 },
        { category: 'Others', budget: 100, spent: 0 },
      ],
      expenses: [
        {
          id: 'nyc-1',
          date: '2026-08-01',
          description: 'Broadway ticket',
          note: 'Deposit',
          category: 'Activities',
          paymentMethod: 'Credit Card',
          amount: 70,
        },
      ],
    },
  ];

  getTrips(): Observable<TripExpenseContext[]> {
    return of(this.tripStores.map((entry) => entry.trip));
  }

  getDashboardData(tripId: string): Observable<ExpenseDashboardData> {
    const store =
      this.tripStores.find((entry) => entry.trip.id === tripId) ?? this.tripStores[0];

    return of({
      summary: { ...store.summary },
      categories: store.categories.map((category) => ({ ...category })),
      budgetComparison: store.budgetComparison.map((item) => ({ ...item })),
      expenses: store.expenses.map((expense) => ({ ...expense })),
    });
  }

  addExpense(tripId: string, payload: AddExpensePayload): Observable<ExpenseItem> {
    const store =
      this.tripStores.find((entry) => entry.trip.id === tripId) ?? this.tripStores[0];

    const item: ExpenseItem = {
      id: `exp-${Date.now()}`,
      ...payload,
    };

    store.expenses = [item, ...store.expenses];

    store.summary.totalSpent += payload.amount;
    store.summary.remaining = Math.max(store.summary.totalBudget - store.summary.totalSpent, 0);
    store.summary.dailyAverage = Number((store.summary.totalSpent / store.trip.days).toFixed(2));

    const category = store.categories.find((entry) => entry.category === payload.category);
    if (category) {
      category.amount += payload.amount;
    }

    const budgetItem = store.budgetComparison.find((entry) => entry.category === payload.category);
    if (budgetItem) {
      budgetItem.spent += payload.amount;
    }

    return of({ ...item });
  }
}
