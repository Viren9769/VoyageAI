import { Injectable } from '@angular/core';

import { AppConstants } from '../constants/app.constants';
import { AppPreferences, UserPreferences, UserProfile } from '../../models/settings';

const PREFS_KEY = 'voyage_user_preferences';
const APP_PREFS_KEY = 'voyage_app_preferences';

@Injectable({ providedIn: 'root' })
export class UserProfileService {

  getProfile(): UserProfile {
    const raw =
      localStorage.getItem(AppConstants.Storage.CurrentUser) ??
      sessionStorage.getItem(AppConstants.Storage.CurrentUser);

    const base = raw ? JSON.parse(raw) : {};

    return {
      userId: base.userId ?? '',
      firstName: base.firstName ?? '',
      lastName: base.lastName ?? '',
      email: base.email ?? '',
      phone: base.phone ?? '',
      dateOfBirth: base.dateOfBirth ?? '',
      bio: base.bio ?? '',
      profileImageUrl: base.profileImageUrl,
    };
  }

  saveProfile(profile: UserProfile): void {
    const storage = localStorage.getItem(AppConstants.Storage.CurrentUser)
      ? localStorage
      : sessionStorage;
    storage.setItem(AppConstants.Storage.CurrentUser, JSON.stringify(profile));
  }

  getPreferences(): UserPreferences {
    const raw = localStorage.getItem(PREFS_KEY);
    if (raw) return JSON.parse(raw);
    return {
      currency: 'USD - US Dollar',
      language: 'English',
      dateFormat: 'MM/DD/YYYY',
      timeFormat: '12 Hour (AM/PM)',
      theme: 'Dark',
    };
  }

  savePreferences(prefs: UserPreferences): void {
    localStorage.setItem(PREFS_KEY, JSON.stringify(prefs));
  }

  getAppPreferences(): AppPreferences {
    const raw = localStorage.getItem(APP_PREFS_KEY);
    if (raw) return JSON.parse(raw);
    return {
      smartRecommendations: true,
      autoItineraryOptimization: true,
      offlineAccess: true,
      priceAlerts: false,
    };
  }

  saveAppPreferences(prefs: AppPreferences): void {
    localStorage.setItem(APP_PREFS_KEY, JSON.stringify(prefs));
  }
}
