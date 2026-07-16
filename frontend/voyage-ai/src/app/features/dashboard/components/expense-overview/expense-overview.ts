import { Component, Input } from '@angular/core';

import { CommonModule } from '@angular/common';

import { MatIconModule } from '@angular/material/icon';

import {ExpenseOverview} from '../../../../models/dashboard';

@Component({
  selector: 'app-expense-overview',

  standalone: true,

  imports: [
    CommonModule,
    MatIconModule
  ],

  templateUrl: './expense-overview.html',

  styleUrl: './expense-overview.scss'
})
export class ExpenseOverviewComponent {

  @Input({ required: true })

  expense!: ExpenseOverview;

}