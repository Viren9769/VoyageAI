import { Component, EventEmitter, Output } from '@angular/core';
import { MatIconModule } from '@angular/material/icon';

@Component({
  selector: 'app-account-actions',
  standalone: true,
  imports: [MatIconModule],
  templateUrl: './account-actions.html',
  styleUrl: './account-actions.scss',
})
export class AccountActions {
  @Output() downloadData = new EventEmitter<void>();
  @Output() deleteAccount = new EventEmitter<void>();
  @Output() logOut = new EventEmitter<void>();
}
