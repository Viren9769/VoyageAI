import { inject } from '@angular/core';
import {
  CanActivateChildFn,
  CanActivateFn,
  CanMatchFn,
  RedirectCommand,
  Router,
  UrlSegment
} from '@angular/router';

import { TokenService } from '../authentication/token.service';

const resolveAuth = (returnUrl: string) => {

  const tokenService = inject(TokenService);
  const router = inject(Router);

  if (tokenService.isLoggedIn()) {
    return true;
  }

  return new RedirectCommand(
    router.createUrlTree(
      ['/login'],
      {
        queryParams: {
          returnUrl
        }
      }
    ),
    {
      replaceUrl: true
    }
  );

};

const getMatchUrl = (segments: UrlSegment[]): string => {

  const path = segments
    .map(segment => segment.path)
    .join('/');

  return path ? `/${path}` : '/';

};

const resolveGuest = () => {

  const tokenService = inject(TokenService);
  const router = inject(Router);

  if (!tokenService.isLoggedIn()) {
    return true;
  }

  return new RedirectCommand(
    router.createUrlTree(
      ['/dashboard']
    ),
    {
      replaceUrl: true
    }
  );

};

export const authGuard: CanActivateFn = (_route, state) => {
  return resolveAuth(state.url);
};

export const authChildGuard: CanActivateChildFn = (_childRoute, state) => {
  return resolveAuth(state.url);
};

export const authMatchGuard: CanMatchFn = (_route, segments) => {
  return resolveAuth(getMatchUrl(segments));
};

export const guestGuard: CanActivateFn = () => {
  return resolveGuest();
};

export const guestMatchGuard: CanMatchFn = () => {
  return resolveGuest();
};