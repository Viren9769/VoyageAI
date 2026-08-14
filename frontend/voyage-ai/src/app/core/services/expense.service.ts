import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, map, tap } from 'rxjs';

import { ApiConfig } from '../configuration/api.config';

import {
  AddExpensePayload,
  BudgetComparisonItem,
  CategorySpend,
  ExpenseDashboardData,
  ExpenseItem,
  ExpenseSummary,
  TripExpenseContext,
} from '../../models/expense';

interface GetTripResponse {
  tripId: string;
  tripName: string;
  destinationCountry: string;
  startDate: string;
  endDate: string;
  budget: number;
  coverImageUrl?: string | null;
  status: string;
}

interface ExpenseResponse {
  expenseId: string;
  tripId: string;
  expenseDate: string;
  description: string;
  note: string;
  category: string;
  paymentMethod: string;
  amount: number;
}

interface ExpenseDashboardResponse {
  summary: {
    totalBudget: number;
    totalSpent: number;
    remaining: number;
    dailyAverage: number;
  };
  categories: Array<{ category: string; amount: number; color: string }>;
  budgetComparison: Array<{ category: string; budget: number; spent: number }>;
  expenses: ExpenseResponse[];
}

@Injectable({
  providedIn: 'root',
})
export class ExpenseService {

  private readonly http = inject(HttpClient);
  private readonly apiUrl = ApiConfig.baseUrl + ApiConfig.trips.base;

  getTrips(): Observable<TripExpenseContext[]> {
    return this.http.get<GetTripResponse[]>(this.apiUrl).pipe(
      map(trips => trips.map(trip => this.mapTrip(trip)))
    );
  }

  getDashboardData(tripId: string): Observable<ExpenseDashboardData> {
    return this.http.get<ExpenseDashboardResponse>(`${this.apiUrl}/${tripId}/expenses/dashboard`).pipe(
      map(response => ({
        summary: response.summary,
        categories: response.categories.map(category => this.mapCategory(category)),
        budgetComparison: response.budgetComparison.map(item => ({
          category: this.normalizeBudgetCategory(item.category),
          budget: item.budget,
          spent: item.spent,
        })),
        expenses: response.expenses.map(expense => this.mapExpense(expense))
      }))
    );
  }

  addExpense(tripId: string, payload: AddExpensePayload): Observable<ExpenseItem> {
    const request = {
      expenseDate: payload.date,
      description: payload.description,
      note: payload.note,
      category: payload.category,
      paymentMethod: payload.paymentMethod,
      amount: payload.amount
    };

    return this.http.post<{ data?: ExpenseResponse } | ExpenseResponse>(`${this.apiUrl}/${tripId}/expenses`, request).pipe(
      map(response => this.extractExpense(response)),
      map(expense => this.mapExpense(expense))
    );
  }

  private mapTrip(response: GetTripResponse): TripExpenseContext {
    const startDate = new Date(response.startDate);
    const endDate = new Date(response.endDate);
    const days = Math.max(1, Math.ceil((endDate.getTime() - startDate.getTime()) / (1000 * 60 * 60 * 24)) + 1);

    return {
      id: response.tripId,
      title: response.tripName,
      startDate: startDate.toISOString().slice(0, 10),
      endDate: endDate.toISOString().slice(0, 10),
      days,
      status: this.normalizeStatus(response.status),
      budget: response.budget,
      coverImage: response.coverImageUrl || 'images/default-trip.jpg'
    };
  }

  private mapExpense(response: ExpenseResponse): ExpenseItem {
    return {
      id: response.expenseId,
      date: response.expenseDate.slice(0, 10),
      description: response.description,
      note: response.note,
      category: this.normalizeCategory(response.category),
      paymentMethod: this.normalizePaymentMethod(response.paymentMethod),
      amount: response.amount,
    };
  }

  private mapCategory(response: { category: string; amount: number; color: string }): CategorySpend {
    return {
      category: this.normalizeCategory(response.category),
      amount: response.amount,
      color: response.color,
    };
  }

  private normalizeBudgetCategory(category: string): BudgetComparisonItem['category'] {
    switch (category.toLowerCase()) {
      case 'accommodation':
        return 'Accommodation';
      case 'transportation':
      case 'transport':
        return 'Transportation';
      case 'food & dining':
      case 'food':
        return 'Food & Dining';
      case 'activities':
      case 'activity':
        return 'Activities';
      case 'shopping':
        return 'Shopping';
      default:
        return 'Others';
    }
  }

  private extractExpense(response: { data?: ExpenseResponse } | ExpenseResponse): ExpenseResponse {
    if ('data' in response && response.data) {
      return response.data;
    }

    return response as ExpenseResponse;
  }

  private normalizeStatus(status: string): TripExpenseContext['status'] {
    switch (status.toLowerCase()) {
      case 'active':
      case 'confirmed':
        return 'Active';
      case 'completed':
        return 'Completed';
      default:
        return 'Draft';
    }
  }

  private normalizeCategory(category: string): CategorySpend['category'] {
    switch (category.toLowerCase()) {
      case 'accommodation':
        return 'Accommodation';
      case 'transportation':
      case 'transport':
        return 'Transportation';
      case 'food & dining':
      case 'food':
        return 'Food & Dining';
      case 'activities':
      case 'activity':
        return 'Activities';
      case 'shopping':
        return 'Shopping';
      default:
        return 'Others';
    }
  }

  private normalizePaymentMethod(method: string): ExpenseItem['paymentMethod'] {
    switch (method.toLowerCase()) {
      case 'cash':
        return 'Cash';
      case 'bank transfer':
        return 'Bank Transfer';
      case 'wallet':
        return 'Wallet';
      default:
        return 'Credit Card';
    }
  }
}
