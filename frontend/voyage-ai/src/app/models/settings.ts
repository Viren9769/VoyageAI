export interface UserProfile {
  userId: string;
  firstName: string;
  lastName: string;
  email: string;
  phone: string;
  dateOfBirth: string;
  bio: string;
  profileImageUrl?: string;
}

export interface UserPreferences {
  currency: string;
  language: string;
  dateFormat: string;
  timeFormat: string;
  theme: string;
}

export interface AppPreferences {
  smartRecommendations: boolean;
  autoItineraryOptimization: boolean;
  offlineAccess: boolean;
  priceAlerts: boolean;
}

export type SettingsTab = 'profile' | 'preferences' | 'notifications' | 'security' | 'billing' | 'privacy';
