import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';

import {
  FormBuilder,
  ReactiveFormsModule,
  Validators
} from '@angular/forms';

import { Router, RouterLink } from '@angular/router';

import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatIconModule } from '@angular/material/icon';

import { finalize } from 'rxjs';

import { AuthService } from '../../../core/services/auth.service';
import { TokenService } from '../../../core/authentication/token.service';

import { LoginRequest } from '../../../models/auth/login-request';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    RouterLink,

    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatCheckboxModule,
    MatIconModule
  ],
  templateUrl: './login.html',
  styleUrl: './login.scss'
})
export class Login {

  //==================================================
  // Dependency Injection
  //==================================================

  private readonly fb = inject(FormBuilder);

  private readonly router = inject(Router);

  private readonly authService = inject(AuthService);

  private readonly tokenService = inject(TokenService);

  //==================================================
  // Signals
  //==================================================

  hidePassword = signal(true);

  loading = signal(false);

  errorMessage = signal('');

  //==================================================
  // Login Form
  //==================================================

  loginForm = this.fb.nonNullable.group({

    email: [
      '',
      [
        Validators.required,
        Validators.email
      ]
    ],

    password: [
      '',
      [
        Validators.required,
        Validators.minLength(8)
      ]
    ],

    rememberMe: [true]

  });

  //==================================================
  // Toggle Password
  //==================================================

  togglePassword(): void {

    this.hidePassword.update(value => !value);

  }

  //==================================================
  // Login
  //==================================================

  login(): void {

    this.errorMessage.set('');

    if (this.loginForm.invalid) {

      this.loginForm.markAllAsTouched();

      return;

    }

    this.loading.set(true);

    const request: LoginRequest = {

      email: this.loginForm.controls.email.value,

      password: this.loginForm.controls.password.value

    };

    this.authService
      .login(request)
      .pipe(
        finalize(() => this.loading.set(false))
      )
      .subscribe({

        next: (response) => {

          //==================================
          // Save User
          //==================================

          localStorage.setItem(
            'voyage_user',
            JSON.stringify(response.user)
          );

          //==================================
          // Save Tokens
          //==================================

          this.tokenService.saveTokens(
            response.accessToken,
            response.refreshToken,
            this.loginForm.controls.rememberMe.value
          );

          console.log('Login Successful');

          console.log(response);

          this.router.navigateByUrl(
            '/dashboard',
            {
              replaceUrl: true
            }
          );

        },

        error: (error) => {

          console.error(error);

          if (error.status === 401) {

            this.errorMessage.set(
              'Invalid email or password.'
            );

          }
          else if (error.status === 400) {

            this.errorMessage.set(
              'Invalid request.'
            );

          }
          else {

            this.errorMessage.set(
              error.error?.message ??
              'Something went wrong. Please try again.'
            );

          }

        }

      });

  }

  //==================================================
  // Google Login
  //==================================================

  loginWithGoogle(): void {

    console.log('Google Login');

  }

  //==================================================
  // Apple Login
  //==================================================

  loginWithApple(): void {

    console.log('Apple Login');

  }

}