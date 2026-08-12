import { Component, EventEmitter, Output } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';

@Component({
  selector: 'app-traveler-header',
  standalone: true,
  imports: [MatButtonModule, MatIconModule],
  templateUrl: './traveler-header.html',
  styleUrl: './traveler-header.scss',
})
export class TravelerHeader {
  @Output() addTraveler = new EventEmitter<void>();
}
