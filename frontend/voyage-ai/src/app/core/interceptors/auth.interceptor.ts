import {
  HttpErrorResponse,
  HttpInterceptorFn,
  HttpRequest
} from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import {
  Observable,
  catchError,
  finalize,
  shareReplay,
  switchMap,
  tap,
  throwError
} from 'rxjs';

import { TokenService } from '../authentication/token.service';
import { AuthService } from '../services/auth.service';
import { ApiConfig } from '../configuration/api.config';
import { LoginResponse } from '../../models/auth/login-response';

let refreshRequest$: Observable<LoginResponse> | null = null;

const withAuthHeader = (
  request: HttpRequest<unknown>,
  token: string
) => {

  return request.clone({
    setHeaders: {
      Authorization: `Bearer ${token}`
    }
  });

};

const isAuthRequest = (url: string): boolean => {

  return [
    ApiConfig.auth.login,
    ApiConfig.auth.register,
    ApiConfig.auth.refreshToken
  ].some(path => url.includes(path));

};

const redirectToLogin = (
  tokenService: TokenService,
  router: Router
) => {

  tokenService.clearTokens();

  router.navigateByUrl(
    '/login',
    {
      replaceUrl: true
    }
  );

};

const refreshAndRetryRequest = (
  request: HttpRequest<unknown>,
  next: Parameters<HttpInterceptorFn>[1],
  authService: AuthService,
  tokenService: TokenService,
  router: Router
) => {

  const refreshToken = tokenService.getRefreshToken();

  if (!refreshToken) {

    redirectToLogin(tokenService, router);

    return throwError(
      () => new Error('No refresh token available')
    );

  }

  if (!refreshRequest$) {

    refreshRequest$ = authService.refreshToken({
      refreshToken
    }).pipe(
      tap(response => {
        tokenService.saveRefreshedTokens(
          response.accessToken,
          response.refreshToken
        );
      }),
      shareReplay(1),
      finalize(() => {
        refreshRequest$ = null;
      })
    );

  }

  return refreshRequest$.pipe(
    switchMap(response => {
      return next(
        withAuthHeader(
          request,
          response.accessToken
        )
      );
    }),
    catchError(error => {
      redirectToLogin(tokenService, router);

      return throwError(() => error);
    })
  );

};

export const authInterceptor: HttpInterceptorFn = (req, next) => {

  const tokenService = inject(TokenService);
  const authService = inject(AuthService);
  const router = inject(Router);

  const token = tokenService.getAccessToken();
  const hasValidAccessToken =
    !!token && !tokenService.isTokenExpired(token);

  const request = hasValidAccessToken
    ? withAuthHeader(req, token)
    : req;

  return next(request).pipe(
    catchError((error: unknown) => {
      if (!(error instanceof HttpErrorResponse)) {
        return throwError(() => error);
      }

      if (error.status !== 401 || isAuthRequest(req.url)) {
        return throwError(() => error);
      }

      return refreshAndRetryRequest(
        req,
        next,
        authService,
        tokenService,
        router
      );
    })
  );

};