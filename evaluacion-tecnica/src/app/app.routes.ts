import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth.guard';
import { evaluadorGuard } from './core/guards/evaluador.guard';
import { candidatoGuard } from './core/guards/candidato.guard';

export const routes: Routes = [
  { path: '', redirectTo: '/login', pathMatch: 'full' },
  {
    path: 'login',
    loadComponent: () => import('./auth/login/login.component').then(m => m.LoginComponent)
  },
  {
    path: 'registro',
    loadComponent: () => import('./auth/registro/registro.component').then(m => m.RegistroComponent)
  },
  {
    path: 'evaluador',
    canActivate: [authGuard, evaluadorGuard],
    loadChildren: () => import('./evaluador/evaluador.routes').then(m => m.EVALUADOR_ROUTES)
  },
  {
    path: 'panel-candidato',
    canActivate: [authGuard, candidatoGuard],
    loadChildren: () => import('./candidato/candidato-dashboard.routes').then(m => m.CANDIDATO_DASHBOARD_ROUTES)
  },
  {
    path: 'candidato/:token',
    loadChildren: () => import('./candidato/candidato.routes').then(m => m.CANDIDATO_ROUTES)
  },
  { path: '**', redirectTo: '/login' }
];
