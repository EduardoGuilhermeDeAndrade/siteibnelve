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
        title: 'Portal Admin — Nova série'
      },
      {
        path: 'agenda/:id/editar',
        loadComponent: () => import('./agenda/evento-form/evento-form').then((m) => m.EventoForm),
        title: 'Portal Admin — Editar série'
      },
      {
        path: 'agenda/:id/dividir/:data',
        loadComponent: () => import('./agenda/evento-form/evento-form').then((m) => m.EventoForm),
        title: 'Portal Admin — Editar esta e as próximas'
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
      },
      {
        path: 'conteudo',
        loadComponent: () => import('./conteudo/conteudo-admin').then((m) => m.ConteudoAdmin),
        title: 'Portal Admin — Conteúdo do Site'
      },
      {
        path: 'ministerios',
        loadComponent: () => import('./ministerios/ministerios-admin').then((m) => m.MinisteriosAdmin),
        title: 'Portal Admin — Ministérios'
      },
      {
        path: 'ministerios/novo',
        loadComponent: () =>
          import('./ministerios/ministerio-form/ministerio-form').then((m) => m.MinisterioForm),
        title: 'Portal Admin — Novo ministério'
      },
      {
        path: 'ministerios/:id/editar',
        loadComponent: () =>
          import('./ministerios/ministerio-form/ministerio-form').then((m) => m.MinisterioForm),
        title: 'Portal Admin — Editar ministério'
      },
      {
        path: 'contribuicoes',
        loadComponent: () =>
          import('./contribuicao/contribuicao-admin').then((m) => m.ContribuicaoAdmin),
        title: 'Portal Admin — Contribuições'
      },
      {
        path: 'usuarios',
        loadComponent: () => import('./usuarios/usuarios-admin').then((m) => m.UsuariosAdmin),
        title: 'Portal Admin — Usuários'
      },
      {
        path: 'usuarios/novo',
        loadComponent: () => import('./usuarios/usuario-form/usuario-form').then((m) => m.UsuarioForm),
        title: 'Portal Admin — Novo usuário'
      },
      {
        path: 'usuarios/:id/editar',
        loadComponent: () => import('./usuarios/usuario-form/usuario-form').then((m) => m.UsuarioForm),
        title: 'Portal Admin — Editar usuário'
      },
      {
        path: 'configuracoes',
        loadComponent: () =>
          import('./configuracoes/configuracoes-admin').then((m) => m.ConfiguracoesAdmin),
        title: 'Portal Admin — Configurações'
      },
      {
        path: 'pedidos-oracao',
        loadComponent: () =>
          import('./pedidos-oracao/pedidos-oracao-admin').then((m) => m.PedidosOracaoAdmin),
        title: 'Portal Admin — Pedidos de Oração'
      }
    ]
  }
];
