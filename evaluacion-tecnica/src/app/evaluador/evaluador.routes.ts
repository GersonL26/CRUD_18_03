import { Routes } from '@angular/router';

export const EVALUADOR_ROUTES: Routes = [
  {
    path: '',
    loadComponent: () => import('./layout/evaluador-layout.component').then(m => m.EvaluadorLayoutComponent),
    children: [
      { path: '', redirectTo: 'dashboard', pathMatch: 'full' },
      {
        path: 'dashboard',
        loadComponent: () => import('./dashboard/dashboard.component').then(m => m.DashboardComponent)
      },
      {
        path: 'crear',
        loadComponent: () => import('./crear-evaluacion/crear-evaluacion.component').then(m => m.CrearEvaluacionComponent)
      },
      {
        path: 'resultados',
        loadComponent: () => import('./resultados-lista/resultados-lista.component').then(m => m.ResultadosListaComponent)
      },
      {
        path: 'rankings',
        loadComponent: () => import('./rankings-lista/rankings-lista.component').then(m => m.RankingsListaComponent)
      },
      {
        path: 'sesiones',
        loadComponent: () => import('./sesiones-lista/sesiones-lista.component').then(m => m.SesionesListaComponent)
      },
      {
        path: 'evaluacion/:id',
        loadComponent: () => import('./detalle-evaluacion/detalle-evaluacion.component').then(m => m.DetalleEvaluacionComponent)
      },
      {
        path: 'evaluacion/:eId/resultado/:cId',
        loadComponent: () => import('./resultado/resultado.component').then(m => m.ResultadoComponent)
      },
      {
        path: 'evaluacion/:eId/ranking',
        loadComponent: () => import('./ranking/ranking.component').then(m => m.RankingComponent)
      },
      {
        path: 'evaluacion/:eId/comparar',
        loadComponent: () => import('./comparar/comparar.component').then(m => m.CompararComponent)
      },
      {
        path: 'evaluacion/:eId/sesion-vivo/:sId',
        loadComponent: () => import('./sesion-vivo/sesion-vivo.component').then(m => m.SesionVivoComponent)
      }
    ]
  }
];
