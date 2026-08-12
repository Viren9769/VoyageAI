import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, OnChanges, Output, SimpleChanges } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MatIconModule } from '@angular/material/icon';

import { UserProfile } from '../../../../models/settings';

@Component({
  selector: 'app-profile-info',
  standalone: true,
  imports: [CommonModule, FormsModule, MatIconModule],
  templateUrl: './profile-info.html',
  styleUrl: './profile-info.scss',
})
export class ProfileInfo implements OnChanges {
  @Input() profile!: UserProfile;
  @Output() profileChange = new EventEmitter<UserProfile>();

  model: UserProfile = this.empty();

  readonly bioMaxLength = 150;

  get bioLength(): number {
    return this.model.bio?.length ?? 0;
  }

  get initials(): string {
    return `${this.model.firstName?.[0] ?? ''}${this.model.lastName?.[0] ?? ''}`.toUpperCase();
  }

  ngOnChanges(_: SimpleChanges): void {
    this.model = { ...this.profile };
  }

  onFieldChange(): void {
    this.profileChange.emit({ ...this.model });
  }

  private empty(): UserProfile {
    return { userId: '', firstName: '', lastName: '', email: '', phone: '', dateOfBirth: '', bio: '' };
  }
}
