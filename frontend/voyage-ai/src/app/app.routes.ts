import { Routes } from '@angular/router';

import { authGuard } from './core/guards/auth.guard';
import {MainLayout} from './layouts/main-layout/main-layout';

export const routes: Routes = [

  {
    path: '',
    redirectTo: 'login',
    pathMatch: 'full'
  },

  {
    path: 'login',
    loadComponent: () =>
      import('./features/auth/login/login')
        .then(c => c.Login)
  },

  {
    path: 'register',
    loadComponent: () =>
      import('./features/auth/register/register')
        .then(c => c.Register)
  },


  //-------------------------------------------------------
  // Main Layout
  //-------------------------------------------------------

  {
    path: '',
    component: MainLayout,
    canActivate: [authGuard],

    children: [

      {
        path: 'dashboard',
        loadComponent: () =>
          import('./features/dashboard/dashboard/dashboard')
            .then(c => c.Dashboard)
      },

      // {
      //   path: 'trips',
      //   loadComponent: () =>
      //     import('./features/trips/trips')
      //       .then(c => c.Trips)
      // },

      // {
      //   path: 'itinerary',
      //   loadComponent: () =>
      //     import('./features/itinerary/itinerary')
      //       .then(c => c.Itinerary)
      // },

      // {
      //   path: 'expenses',
      //   loadComponent: () =>
      //     import('./features/expenses/expenses')
      //       .then(c => c.Expenses)
      // },

      // {
      //   path: 'travelers',
      //   loadComponent: () =>
      //     import('./features/travelers/travelers')
      //       .then(c => c.Travelers)
      // },

      // {
      //   path: 'documents',
      //   loadComponent: () =>
      //     import('./features/documents/documents')
      //       .then(c => c.Documents)
      // },

      // {
      //   path: 'settings',
      //   loadComponent: () =>
      //     import('./features/settings/settings')
      //       .then(c => c.Settings)
      // }

    ]

  },

  //-------------------------------------------------------
  // Unknown Route
  //-------------------------------------------------------

  {
    path: '**',
    redirectTo: 'login'
  }

];


//Test