import { Routes } from '@angular/router';

import {
  authChildGuard,
  authGuard,
  authMatchGuard,
  guestGuard,
  guestMatchGuard
} from './core/guards/auth.guard';
import {MainLayout} from './layouts/main-layout/main-layout';
export const routes: Routes = [

  {
    path: '',
    redirectTo: 'login',
    pathMatch: 'full'
  },

  {
    path: 'login',
    canMatch: [guestMatchGuard],
    canActivate: [guestGuard],
    loadComponent: () =>
      import('./features/auth/login/login')
        .then(c => c.Login)
  },

  {
    path: 'register',
    canMatch: [guestMatchGuard],
    canActivate: [guestGuard],
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
    canMatch: [authMatchGuard],
    canActivate: [authGuard],
    canActivateChild: [authChildGuard],
    runGuardsAndResolvers: 'always',

    children: [

      {
        path: 'dashboard',
        loadComponent: () =>
          import('./features/dashboard/dashboard/dashboard')
            .then(c => c.Dashboard)
      },

       {
        path: 'trips',
         loadComponent: () =>
         import('./features/trips/trips/trips')
             .then(c => c.Trips)
       },
       {
    path:'trips/create',
    loadComponent:()=>
        import('./features/create-trip/create-trip')
        .then(c=>c.CreateTripComponent)
},

      {
        path: 'itinerary',
        loadComponent: () =>
          import('./features/itinerary/itinerary/itinerary')
            .then(c => c.Itinerary)
      },
      
      {
        path: 'aiplanner',
        loadComponent: () =>
          import('./features/aiplanner/aiplanner')
            .then(c => c.Aiplanner)
      },

      {
        path: 'planner',
        redirectTo: 'aiplanner',
        pathMatch: 'full'
      },


      {
        path: 'expenses',
        loadComponent: () =>
          import('./features/expenses/expenses')
            .then(c => c.Expenses)
      },

      {
        path: 'travelers',
        loadComponent: () =>
          import('./features/travelers/travelers')
            .then(c => c.Travelers)
      },

      {
        path: 'settings',
        loadComponent: () =>
          import('./features/settings/settings')
            .then(c => c.Settings)
      },

      {
        path: '**',
        redirectTo: 'dashboard'
      }

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