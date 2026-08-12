import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';

import { UserProfileService } from '../../core/services/user-profile.service';
import { TokenService } from '../../core/authentication/token.service';
import { AppPreferences, SettingsTab, UserPreferences, UserProfile } from '../../models/settings';
import { ProfileInfo } from './components/profile-info/profile-info';
import { PreferencesPanel } from './components/preferences-panel/preferences-panel';
import { AppPreferencesPanel } from './components/app-preferences/app-preferences';
import { AccountActions } from './components/account-actions/account-actions';

@Component({
  selector: 'app-settings',
  standalone: true,
  imports: [CommonModule, MatButtonModule, MatIconModule, ProfileInfo, PreferencesPanel, AppPreferencesPanel, AccountActions],
  templateUrl: './settings.html',
  styleUrl: './settings.scss',
})
export class Settings implements OnInit {
  activeTab: SettingsTab = 'profile';

  profile!: UserProfile;
  prefs!: UserPreferences;
  appPrefs!: AppPreferences;

  saved = false;

  readonly tabs: { id: SettingsTab; label: string }[] = [
    { id: 'profile', label: 'Profile' },
    { id: 'preferences', label: 'Preferences' },
    { id: 'notifications', label: 'Notifications' },
    { id: 'security', label: 'Security' },
    { id: 'billing', label: 'Billing & Plan' },
    { id: 'privacy', label: 'Data & Privacy' },
  ];

  constructor(
    private readonly profileService: UserProfileService,
    private readonly tokenService: TokenService,
    private readonly router: Router,
  ) {}

  ngOnInit(): void {
    this.profile = this.profileService.getProfile();
    this.prefs = this.profileService.getPreferences();
    this.appPrefs = this.profileService.getAppPreferences();
  }

  setTab(tab: SettingsTab): void {
    this.activeTab = tab;
  }

  onProfileChange(p: UserProfile): void {
    this.profile = p;
  }

  onPrefsChange(p: UserPreferences): void {
    this.prefs = p;
  }

  onAppPrefsChange(p: AppPreferences): void {
    this.appPrefs = p;
  }

  saveChanges(): void {
    this.profileService.saveProfile(this.profile);
    this.profileService.savePreferences(this.prefs);
    this.profileService.saveAppPreferences(this.appPrefs);
    this.saved = true;
    setTimeout(() => (this.saved = false), 2500);
  }

  onDownloadData(): void {
    const data = { profile: this.profile, preferences: this.prefs, appPreferences: this.appPrefs };
    const blob = new Blob([JSON.stringify(data, null, 2)], { type: 'application/json' });
    const url = URL.createObjectURL(blob);
    const a = document.createElement('a');
    a.href = url;
    a.download = 'voyageai-data.json';
    a.click();
    URL.revokeObjectURL(url);
  }

  onDeleteAccount(): void {
    // TODO: confirm dialog before real deletion
    this.onLogOut();
  }

  onLogOut(): void {
    this.tokenService.clearTokens();
    localStorage.removeItem('voyage_current_user');
    sessionStorage.removeItem('voyage_current_user');
    this.router.navigateByUrl('/login', { replaceUrl: true });
  }
}
