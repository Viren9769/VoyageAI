import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, OnChanges, Output, SimpleChanges } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MatIconModule } from '@angular/material/icon';

import { TravelerFormPayload, TravelerType, Traveler } from '../../../../models/traveler';
import { TripOption } from '../../../../core/services/traveler.service';

@Component({
  selector: 'app-traveler-form',
  standalone: true,
  imports: [CommonModule, FormsModule, MatIconModule],
  templateUrl: './traveler-form.html',
  styleUrl: './traveler-form.scss',
})
export class TravelerForm implements OnChanges {
  @Input() isOpen = false;
  @Input() editTarget: Traveler | null = null;
  @Input() tripOptions: TripOption[] = [];

  @Output() close = new EventEmitter<void>();
  @Output() save = new EventEmitter<TravelerFormPayload>();

  readonly types: TravelerType[] = ['Adult', 'Child'];

  model: TravelerFormPayload = this.defaultModel();

  get title(): string {
    return this.editTarget ? 'Edit Traveler' : 'Add Traveler';
  }

  get assignableTripOptions(): TripOption[] {
    return this.tripOptions.filter((t) => t.id !== 'all');
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['isOpen'] && this.isOpen) {
      this.model = this.editTarget
        ? {
            firstName: this.editTarget.firstName,
            lastName: this.editTarget.lastName,
            email: this.editTarget.email,
            phone: this.editTarget.phone,
            type: this.editTarget.type,
            age: this.editTarget.age,
            assignedTripIds: [...this.editTarget.assignedTripIds],
          }
        : this.defaultModel();
    }
  }

  isTripSelected(id: string): boolean {
    return this.model.assignedTripIds.includes(id);
  }

  toggleTrip(id: string): void {
    if (this.isTripSelected(id)) {
      this.model.assignedTripIds = this.model.assignedTripIds.filter((t) => t !== id);
    } else {
      this.model.assignedTripIds = [...this.model.assignedTripIds, id];
    }
  }

  onSave(): void {
    if (!this.model.firstName.trim() || !this.model.lastName.trim() || !this.model.email.trim()) {
      return;
    }
    const payload: TravelerFormPayload = { ...this.model };
    if (payload.type === 'Adult') {
      delete payload.age;
    }
    this.save.emit(payload);
  }

  onClose(): void {
    this.model = this.defaultModel();
    this.close.emit();
  }

  private defaultModel(): TravelerFormPayload {
    return {
      firstName: '',
      lastName: '',
      email: '',
      phone: '',
      type: 'Adult',
      assignedTripIds: [],
    };
  }
}
