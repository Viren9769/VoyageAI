import { Component, Input } from '@angular/core';

import { CommonModule } from '@angular/common';

import { TravelMap } from '../../../../models/dashboard';
import {MatIconModule} from "@angular/material/icon";
import { MatButtonModule } from '@angular/material/button';

@Component({

  selector: 'app-travel-map',

  standalone: true,

  imports: [
    CommonModule,
    MatIconModule,
    MatButtonModule
  ],

  templateUrl: './travel-map.html',

  styleUrl: './travel-map.scss'

})

export class TravelMapComponent {

  @Input({ required: true })

  travelMap!: TravelMap;

}