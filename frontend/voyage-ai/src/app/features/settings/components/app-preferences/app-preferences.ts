import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, OnChanges, Output, SimpleChanges } from '@angular/core';
import { FormsModule } from '@angular/forms';

import { AppPreferences } from '../../../../models/settings';

interface Toggle {
  key: keyof AppPreferences;
  label: string;
  description: string;
}

@Component({
  selector: 'app-app-preferences',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './app-preferences.html',
  styleUrl: './app-preferences.scss',
})
export class AppPreferencesPanel implements OnChanges {
  @Input() prefs!: AppPreferences;
  @Output() prefsChange = new EventEmitter<AppPreferences>();

  model: AppPreferences = this.defaults();

  readonly toggles: Toggle[] = [
    { key: 'smartRecommendations', label: 'Smart Recommendations', description: 'AI-powered travel suggestions' },
    { key: 'autoItineraryOptimization', label: 'Auto Itinerary Optimization', description: 'Automatically optimize your itinerary' },
    { key: 'offlineAccess', label: 'Offline Access', description: 'Download trips for offline access' },
    { key: 'priceAlerts', label: 'Price Alerts', description: 'Get notified about price changes' },
  ];

  ngOnChanges(_: SimpleChanges): void {
    this.model = { ...this.prefs };
  }

  toggle(key: keyof AppPreferences): void {
    const m = this.model as unknown as Record<string, boolean>;
    m[key] = !m[key];
    this.prefsChange.emit({ ...this.model });
  }

  private defaults(): AppPreferences {
    return { smartRecommendations: true, autoItineraryOptimization: true, offlineAccess: true, priceAlerts: false };
  }
}
