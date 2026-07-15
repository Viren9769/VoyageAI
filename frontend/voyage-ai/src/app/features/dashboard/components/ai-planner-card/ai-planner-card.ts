import { Component, Input } from '@angular/core';

import { CommonModule } from '@angular/common';

import { FormsModule } from '@angular/forms';

import { MatButtonModule } from '@angular/material/button';

import { MatIconModule } from '@angular/material/icon';

import { AiPlanner } from '../../../../models/dashboard';

@Component({
  selector: 'app-ai-planner-card',

  standalone: true,

  imports: [
    CommonModule,
    FormsModule,
    MatButtonModule,
    MatIconModule
  ],

  templateUrl: './ai-planner-card.html',

  styleUrl: './ai-planner-card.scss'
})
export class AiPlannerCard {

  @Input({ required: true })
  planner!: AiPlanner;

  prompt = '';

  generateTrip(): void {

    console.log(this.prompt);

  }

}