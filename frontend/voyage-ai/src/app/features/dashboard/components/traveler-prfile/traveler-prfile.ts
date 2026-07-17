import { Component, Input } from '@angular/core';

import { CommonModule } from '@angular/common';

import { MatIconModule } from '@angular/material/icon';

import { TravelerProfile } from '../../../../models/dashboard';

@Component({

  selector: 'app-traveler-profile',

  standalone: true,

  imports: [
    CommonModule,
    MatIconModule
  ],

  templateUrl: './traveler-prfile.html',

  styleUrl: './traveler-prfile.scss'

})

export class TravelerPrfile {

  @Input({ required: true })

  profile!: TravelerProfile;

}