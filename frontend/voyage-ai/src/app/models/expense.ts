export type ExpenseCategory =
  | 'Accommodation'
  | 'Transportation'
  | 'Food & Dining'
  | 'Activities'
  | 'Shopping'
  | 'Others';

export type PaymentMethod = 'Credit Card' | 'Cash' | 'Bank Transfer' | 'Wallet';

export type TripProgressStatus = 'Active' | 'Completed' | 'Draft';

export interface TripExpenseContext {
  id: string;
  title: string;
  startDate: string;
  endDate: string;
  days: number;
  status: TripProgressStatus;
  budget: number;
  coverImage?: string;
}

export interface ExpenseItem {
  id: string;
  date: string;
  description: string;
  note: string;
  category: ExpenseCategory;
  paymentMethod: PaymentMethod;
  amount: number;
}

export interface CategorySpend {
  category: ExpenseCategory;
  amount: number;
  color: string;
}

export interface BudgetComparisonItem {
  category: ExpenseCategory;
  budget: number;
  spent: number;
}

export interface ExpenseSummary {
  totalBudget: number;
  totalSpent: number;
  remaining: number;
  dailyAverage: number;
}

export interface AddExpensePayload {
  date: string;
  description: string;
  note: string;
  category: ExpenseCategory;
  paymentMethod: PaymentMethod;
  amount: number;
}

export interface ExpenseDashboardData {
  summary: ExpenseSummary;
  categories: CategorySpend[];
  budgetComparison: BudgetComparisonItem[];
  expenses: ExpenseItem[];
}
