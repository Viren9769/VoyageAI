import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, Output } from '@angular/core';
import { FormsModule } from '@angular/forms';

import { AddExpensePayload, ExpenseCategory, PaymentMethod } from '../../../../models/expense';

@Component({
  selector: 'app-add-expense',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './add-expense.html',
  styleUrl: './add-expense.scss',
})
export class AddExpense {
  @Input() isOpen = false;

  @Output() close = new EventEmitter<void>();
  @Output() save = new EventEmitter<AddExpensePayload>();

  readonly categories: ExpenseCategory[] = [
    'Accommodation',
    'Transportation',
    'Food & Dining',
    'Activities',
    'Shopping',
    'Others',
  ];

  readonly paymentMethods: PaymentMethod[] = ['Credit Card', 'Cash', 'Bank Transfer', 'Wallet'];

  model: AddExpensePayload = this.defaultModel();

  onSave(): void {
    if (!this.model.description.trim() || this.model.amount <= 0) {
      return;
    }

    this.save.emit({ ...this.model });
    this.model = this.defaultModel();
  }

  onClose(): void {
    this.model = this.defaultModel();
    this.close.emit();
  }

  private defaultModel(): AddExpensePayload {
    return {
      date: new Date().toISOString().split('T')[0],
      description: '',
      note: '',
      category: 'Food & Dining',
      paymentMethod: 'Credit Card',
      amount: 0,
    };
  }
}
