import { Routes } from '@angular/router';

export const CANDIDATO_ROUTES: Routes = [
  {
    path: '',
    loadComponent: () => import('./instrucciones/instrucciones.component').then(m => m.InstruccionesComponent)
  },
  {
    path: 'responder',
    loadComponent: () => import('./responder/responder.component').then(m => m.ResponderComponent)
  },
  {
    path: 'resultado',
    loadComponent: () => import('./mi-resultado/mi-resultado.component').then(m => m.MiResultadoComponent)
  },
  {
    path: 'sesion-vivo/:sId',
    loadComponent: () => import('./sesion-vivo-candidato/sesion-vivo-candidato.component').then(m => m.SesionVivoCandidatoComponent)
  }
];
