import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, OnChanges, Output, SimpleChanges } from '@angular/core';
import { FormsModule } from '@angular/forms';

import { UserPreferences } from '../../../../models/settings';

@Component({
  selector: 'app-preferences-panel',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './preferences-panel.html',
  styleUrl: './preferences-panel.scss',
})
export class PreferencesPanel implements OnChanges {
  @Input() prefs!: UserPreferences;
  @Output() prefsChange = new EventEmitter<UserPreferences>();

  model: UserPreferences = this.defaults();

  readonly currencies = ['USD - US Dollar', 'EUR - Euro', 'GBP - British Pound', 'INR - Indian Rupee', 'JPY - Japanese Yen', 'AUD - Australian Dollar'];
  readonly languages = ['English', 'Spanish', 'French', 'German', 'Hindi', 'Japanese'];
  readonly dateFormats = ['MM/DD/YYYY', 'DD/MM/YYYY', 'YYYY-MM-DD'];
  readonly timeFormats = ['12 Hour (AM/PM)', '24 Hour'];
  readonly themes = ['Dark', 'Light', 'System'];

  ngOnChanges(_: SimpleChanges): void {
    this.model = { ...this.prefs };
  }

  onFieldChange(): void {
    this.prefsChange.emit({ ...this.model });
  }

  private defaults(): UserPreferences {
    return { currency: 'USD - US Dollar', language: 'English', dateFormat: 'MM/DD/YYYY', timeFormat: '12 Hour (AM/PM)', theme: 'Dark' };
  }
}
