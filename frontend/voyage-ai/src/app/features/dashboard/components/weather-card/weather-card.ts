import { Component, Input } from '@angular/core';

import { CommonModule } from '@angular/common';

import { MatIconModule } from '@angular/material/icon';

import { WeatherCard as Weather } from '../../../../models/dashboard';

@Component({
  selector: 'app-weather-card',

  standalone: true,

  imports: [
    CommonModule,
    MatIconModule
  ],

  templateUrl: './weather-card.html',

  styleUrl: './weather-card.scss'
})
export class WeatherCard {

  @Input({ required: true })

  weather!: Weather;

}