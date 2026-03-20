import { Routes } from '@angular/router';

export const CANDIDATO_DASHBOARD_ROUTES: Routes = [
  {
    path: '',
    loadComponent: () => import('./layout/candidato-layout.component').then(m => m.CandidatoLayoutComponent),
    children: [
      { path: '', redirectTo: 'mis-evaluaciones', pathMatch: 'full' },
      {
        path: 'mis-evaluaciones',
        loadComponent: () => import('./mis-evaluaciones/mis-evaluaciones.component').then(m => m.MisEvaluacionesComponent)
      },
      {
        path: 'evaluacion/:token',
        loadComponent: () => import('./instrucciones/instrucciones.component').then(m => m.InstruccionesComponent)
      },
      {
        path: 'evaluacion/:token/resultado',
        loadComponent: () => import('./mi-resultado/mi-resultado.component').then(m => m.MiResultadoComponent)
      }
    ]
  }
];
