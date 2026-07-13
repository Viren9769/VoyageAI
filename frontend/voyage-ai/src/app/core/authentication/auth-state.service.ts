import { Injectable, signal } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class AuthStateService {

  isAuthenticated = signal(false);

  setAuthenticated(value: boolean) {
    this.isAuthenticated.set(value);
  }

}