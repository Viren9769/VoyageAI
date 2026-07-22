import { Component } from '@angular/core';

import { CommonModule } from '@angular/common';

import { MatIconModule } from '@angular/material/icon';

import { MatButtonModule } from '@angular/material/button';
import {Router, RouterModule} from '@angular/router';

@Component({
  selector: 'app-hero-header',

  standalone: true,

  imports: [
    CommonModule,
    MatIconModule,
    MatButtonModule,
    RouterModule
  ],

  templateUrl: './hero-header.html',

  styleUrl: './hero-header.scss'
})
export class HeroHeader {
  constructor(private router: Router) {}

  createTrip(): void {

    this.router.navigate(['/trips/create']);

}
}