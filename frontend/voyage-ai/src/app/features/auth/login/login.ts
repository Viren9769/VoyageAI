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

  //==================================
  // Dependency Injection
  //==================================

  private fb = inject(FormBuilder);

  private router = inject(Router);

  //==================================
  // Signals
  //==================================

  hidePassword = signal(true);

  loading = signal(false);

  //==================================
  // Login Form
  //==================================

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

    rememberMe: [false]

  });

  //==================================
  // Toggle Password
  //==================================

  togglePassword(): void {

    this.hidePassword.update(value => !value);

  }

  //==================================
  // Login
  //==================================

  login(): void {

    if (this.loginForm.invalid) {

      this.loginForm.markAllAsTouched();

      return;

    }

    this.loading.set(true);

    console.log('Login Request');

    console.log(this.loginForm.getRawValue());

    // TODO:
    // AuthService.Login()

    setTimeout(() => {

      this.loading.set(false);

      this.router.navigate(['/dashboard']);

    },1000);

  }

  //==================================
  // Google Login
  //==================================

  loginWithGoogle(): void{

    console.log('Google Login');

  }

  //==================================
  // Apple Login
  //==================================

  loginWithApple(): void{

    console.log('Apple Login');

  }

}