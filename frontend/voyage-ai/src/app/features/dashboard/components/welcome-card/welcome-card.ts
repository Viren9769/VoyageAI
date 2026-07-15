import { Component, Input } from '@angular/core';

import { CommonModule } from '@angular/common';

import { WelcomeSection } from '../../../../models/dashboard';

@Component({
  selector: 'app-welcome-card',

  standalone: true,

  imports: [
    CommonModule
  ],

  templateUrl: './welcome-card.html',

  styleUrl: './welcome-card.scss'
})
export class WelcomeCard {

  @Input({ required: true })

  welcome!: WelcomeSection;

}