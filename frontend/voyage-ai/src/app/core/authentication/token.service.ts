import { Injectable } from '@angular/core';
import { jwtDecode } from 'jwt-decode';

import { AppConstants } from '../constants/app.constants';
import { JwtPayload } from '../../models/jwt-payload';

@Injectable({
  providedIn: 'root'
})
export class TokenService {

  saveTokens(
    accessToken: string,
    refreshToken: string,
    rememberMe: boolean = true
  ): void {

    this.clearTokens();

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

  saveRefreshedTokens(
    accessToken: string,
    refreshToken: string
  ): void {

    const storage = this.getCurrentTokenStorage();

    this.clearTokens();

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

    const token = this.getAccessToken();
    const refreshToken = this.getRefreshToken();

    if (!token) {

      return !!refreshToken;

    }

    if (this.isTokenExpired(token)) {

      if (!refreshToken) {

        this.clearTokens();

      }

      return !!refreshToken;

    }

    return true;

  }

  isTokenExpired(token: string): boolean {

    try {

      const payload = jwtDecode<JwtPayload>(token);

      if (!payload.exp) {

        return true;

      }

      const nowInSeconds = Math.floor(Date.now() / 1000);

      return payload.exp <= nowInSeconds;

    }
    catch {

      return true;

    }

  }

  getUser() {

    const token = this.getAccessToken();

    if (!token) {

      return null;

    }

    if (this.isTokenExpired(token)) {
      return null;

    }

    return jwtDecode<JwtPayload>(token);

  }

  private getCurrentTokenStorage(): Storage {

    const localHasTokens =
      !!localStorage.getItem(AppConstants.Storage.AccessToken)
      ||
      !!localStorage.getItem(AppConstants.Storage.RefreshToken);

    if (localHasTokens) {

      return localStorage;

    }

    const sessionHasTokens =
      !!sessionStorage.getItem(AppConstants.Storage.AccessToken)
      ||
      !!sessionStorage.getItem(AppConstants.Storage.RefreshToken);

    if (sessionHasTokens) {

      return sessionStorage;

    }

    return localStorage;

  }

}