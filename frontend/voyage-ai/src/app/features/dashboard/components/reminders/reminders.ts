import { Component, Input } from '@angular/core';

import { CommonModule } from '@angular/common';

import { MatIconModule } from '@angular/material/icon';

import { Reminder } from '../../../../models/dashboard';

@Component({
  selector: 'app-reminders',

  standalone: true,

  imports: [
    CommonModule,
    MatIconModule
  ],

  templateUrl: './reminders.html',

  styleUrl: './reminders.scss'
})
export class Reminders {

  @Input({ required: true })

  reminders!: Reminder[];

}