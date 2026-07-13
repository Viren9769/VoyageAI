import { Injectable } from '@angular/core';

import { AppConstants } from '../constants/app.constants';
import { jwtDecode } from 'jwt-decode';


@Injectable({
  providedIn: 'root'
})
export class TokenService {

  getAccessToken(): string | null {

    return localStorage.getItem(AppConstants.Storage.AccessToken);

  }

  getRefreshToken(): string | null {

    return localStorage.getItem(AppConstants.Storage.RefreshToken);

  }

saveTokens(accessToken: string, refreshToken: string): void {

  localStorage.setItem(
    AppConstants.Storage.AccessToken,
    accessToken
  );

  localStorage.setItem(
    AppConstants.Storage.RefreshToken,
    refreshToken
  );

}

removeTokens(): void {

  localStorage.removeItem(
    AppConstants.Storage.AccessToken
  );

  localStorage.removeItem(
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