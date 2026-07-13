import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { ApiConfig } from '../configuration/api.config';

import { LoginRequest } from '../../models/auth/login-request';
import { RegisterRequest } from '../../models/auth/register-request';
import { LoginResponse } from '../../models/auth/login-response';
import { RefreshTokenRequest } from '../../models/auth/refresh-token-request';

import { ApiResponse } from '../../models/api-response';

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  private readonly http = inject(HttpClient);

  login(request: LoginRequest): Observable<ApiResponse<LoginResponse>> {

    return this.http.post<ApiResponse<LoginResponse>>(
      ApiConfig.baseUrl + ApiConfig.auth.login,
      request
    );

  }

  register(request: RegisterRequest): Observable<ApiResponse<LoginResponse>> {

    return this.http.post<ApiResponse<LoginResponse>>(
      ApiConfig.baseUrl + ApiConfig.auth.register,
      request
    );

  }

  refreshToken(request: RefreshTokenRequest): Observable<ApiResponse<LoginResponse>> {

    return this.http.post<ApiResponse<LoginResponse>>(
      ApiConfig.baseUrl + ApiConfig.auth.refreshToken,
      request
    );

  }

}