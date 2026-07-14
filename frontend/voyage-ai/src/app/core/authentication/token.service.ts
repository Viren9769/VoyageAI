import { Injectable } from '@angular/core';
import { jwtDecode } from 'jwt-decode';

import { AppConstants } from '../constants/app.constants';

@Injectable({
  providedIn: 'root'
})
export class TokenService {

  saveTokens(
    accessToken: string,
    refreshToken: string,
    rememberMe: boolean = true
  ): void {

    const storage = rememberMe
      ? localStorage
      : sessionStorage;

    storage.setItem(
      AppConstants.Storage.AccessToken,
      accessToken
    );

    storage.setItem(
      AppConstants.Storage.RefreshToken,
      refreshToken
    );

  }

  getAccessToken(): string | null {

    return (
      localStorage.getItem(AppConstants.Storage.AccessToken)
      ??
      sessionStorage.getItem(AppConstants.Storage.AccessToken)
    );

  }

  getRefreshToken(): string | null {

    return (
      localStorage.getItem(AppConstants.Storage.RefreshToken)
      ??
      sessionStorage.getItem(AppConstants.Storage.RefreshToken)
    );

  }

  clearTokens(): void {

    localStorage.removeItem(
      AppConstants.Storage.AccessToken
    );

    localStorage.removeItem(
      AppConstants.Storage.RefreshToken
    );

    sessionStorage.removeItem(
      AppConstants.Storage.AccessToken
    );

    sessionStorage.removeItem(
      AppConstants.Storage.RefreshToken
    );

  }

  isLoggedIn(): boolean {

    return !!this.getAccessToken();

  }

  getUser() {

    const token = this.getAccessToken();

    if (!token) {

      return null;

    }

    return jwtDecode(token);

  }

}