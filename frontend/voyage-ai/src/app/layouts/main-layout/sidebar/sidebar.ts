import { Component } from '@angular/core';

import { CommonModule } from '@angular/common';

import {
  Router,
  RouterLink,
  RouterLinkActive
} from '@angular/router';

import { AppConstants } from '../../../core/constants/app.constants';

import { MatIconModule } from '@angular/material/icon';

import { MatButtonModule } from '@angular/material/button';

@Component({
  selector: 'app-sidebar',
  standalone: true,
  imports: [
    CommonModule,
    RouterLink,
    RouterLinkActive,
    MatIconModule,
    MatButtonModule
  ],
  templateUrl: './sidebar.html',
  styleUrl: './sidebar.scss'
})
export class Sidebar {

  userName = '';

  collapsed = false;

  constructor(
    private readonly router: Router
  ) {

    //====================================
    // Load User
    //====================================

    const user =
      localStorage.getItem(AppConstants.Storage.CurrentUser)
      ??
      sessionStorage.getItem(AppConstants.Storage.CurrentUser)
      ??
      localStorage.getItem('voyage_user')
      ??
      sessionStorage.getItem('voyage_user');

    if (user) {

      const currentUser = JSON.parse(user);

      this.userName =
        currentUser.firstName +
        ' ' +
        currentUser.lastName;

    }

    //====================================
    // Restore Sidebar State
    //====================================

    this.collapsed =
      localStorage.getItem('sidebar-collapsed') === 'true';

  }

  //====================================
  // Sidebar Toggle
  //====================================

  toggleSidebar(): void {

    this.collapsed = !this.collapsed;

    localStorage.setItem(
      'sidebar-collapsed',
      String(this.collapsed)
    );

  }

  //====================================
  // Logout
  //====================================

  logout(): void {

    localStorage.clear();

    sessionStorage.clear();

    this.router.navigateByUrl(
      '/login',
      {
        replaceUrl: true
      }
    );

  }

  //====================================
  // Navigation
  //====================================

  menuItems = [

    {
      icon: 'dashboard',
      title: 'Dashboard',
      route: '/dashboard'
    },

    {
      icon: 'flight',
      title: 'Trips',
      route: '/trips'
    },

    {
      icon: 'auto_awesome',
      title: 'AI Planner',
      route: '/aiplanner'
    },

    {
      icon: 'map',
      title: 'Itinerary',
      route: '/itinerary'
    },

    {
      icon: 'payments',
      title: 'Expenses',
      route: '/expenses'
    },

    {
      icon: 'groups',
      title: 'Travelers',
      route: '/travelers'
    },

    {
      icon: 'settings',
      title: 'Settings',
      route: '/settings'
    }

  ];

}