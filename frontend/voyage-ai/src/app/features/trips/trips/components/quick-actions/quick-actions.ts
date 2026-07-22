import { Component } from '@angular/core';

import { CommonModule } from '@angular/common';

import { MatButtonModule } from '@angular/material/button';

import { MatIconModule } from '@angular/material/icon';

@Component({
  selector: 'app-quick-actions',

  standalone: true,

  imports: [
    CommonModule,
    MatButtonModule,
    MatIconModule
  ],

  templateUrl: './quick-actions.html',

  styleUrl: './quick-actions.scss'
})
export class QuickActions {

}