import { Routes } from '@angular/router';

import { adminGuard } from '../core/auth/admin.guard';

export const ADMIN_ROUTES: Routes = [
  {
    path: 'login',
    loadComponent: () => import('./login/login').then((m) => m.Login),
    title: 'Portal Admin — Login'
  },
  {
    path: '',
    loadComponent: () => import('./layout/admin-layout').then((m) => m.AdminLayout),
    canActivate: [adminGuard],
    children: [
      {
        path: '',
        loadComponent: () => import('./dashboard/dashboard').then((m) => m.Dashboard),
        title: 'Portal Admin — Início'
      }
    ]
  }
];
