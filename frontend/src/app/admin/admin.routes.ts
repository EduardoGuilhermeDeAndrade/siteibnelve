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
      },
      {
        path: 'agenda',
        loadComponent: () => import('./agenda/agenda-admin').then((m) => m.AgendaAdmin),
        title: 'Portal Admin — Agenda'
      },
      {
        path: 'agenda/novo',
        loadComponent: () => import('./agenda/evento-form/evento-form').then((m) => m.EventoForm),
        title: 'Portal Admin — Novo evento'
      },
      {
        path: 'agenda/:id/editar',
        loadComponent: () => import('./agenda/evento-form/evento-form').then((m) => m.EventoForm),
        title: 'Portal Admin — Editar evento'
      },
      {
        path: 'locais',
        loadComponent: () => import('./locais/locais-admin').then((m) => m.LocaisAdmin),
        title: 'Portal Admin — Locais'
      },
      {
        path: 'locais/novo',
        loadComponent: () => import('./locais/local-form/local-form').then((m) => m.LocalForm),
        title: 'Portal Admin — Novo local'
      },
      {
        path: 'locais/:id/editar',
        loadComponent: () => import('./locais/local-form/local-form').then((m) => m.LocalForm),
        title: 'Portal Admin — Editar local'
      }
    ]
  }
];
