import { Routes } from '@angular/router';
import { Home } from './pages/home/home';
import { QuemSomos } from './pages/quem-somos/quem-somos';
import { Ministerios } from './pages/ministerios/ministerios';
import { Agenda } from './pages/agenda/agenda';
import { Contato } from './pages/contato/contato';
import { Contribuicoes } from './pages/contribuicoes/contribuicoes';

export const routes: Routes = [
  { path: '', component: Home, title: 'IBNELVE — Início' },
  { path: 'quem-somos', component: QuemSomos, title: 'IBNELVE — Quem Somos' },
  { path: 'ministerios', component: Ministerios, title: 'IBNELVE — Ministérios' },
  { path: 'agenda', component: Agenda, title: 'IBNELVE — Agenda' },
  { path: 'contato', component: Contato, title: 'IBNELVE — Conecte-se' },
  { path: 'contribuicoes', component: Contribuicoes, title: 'IBNELVE — Contribuições' },
  { path: 'admin', loadChildren: () => import('./admin/admin.routes').then((m) => m.ADMIN_ROUTES) },
  { path: '**', redirectTo: '' }
];
