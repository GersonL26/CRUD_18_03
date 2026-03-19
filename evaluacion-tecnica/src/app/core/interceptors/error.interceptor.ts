import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { MatSnackBar } from '@angular/material/snack-bar';
import { catchError, throwError } from 'rxjs';

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const router = inject(Router);
  const snackBar = inject(MatSnackBar);

  return next(req).pipe(
    catchError(error => {
      let mensaje = 'Ocurrió un error inesperado';

      if (error.status === 401) {
        localStorage.removeItem('jwt_token');
        router.navigate(['/login']);
        mensaje = 'Sesión expirada. Inicia sesión nuevamente.';
      } else if (error.status === 403) {
        mensaje = 'No tienes permiso para esta acción.';
      } else if (error.status === 404) {
        mensaje = error.error?.message || 'Recurso no encontrado.';
      } else if (error.status === 400) {
        mensaje = error.error?.message || 'Solicitud inválida.';
      } else if (error.status === 0) {
        mensaje = 'No se pudo conectar con el servidor.';
      }

      snackBar.open(mensaje, 'Cerrar', {
        duration: 4000,
        horizontalPosition: 'end',
        verticalPosition: 'top'
      });

      return throwError(() => error);
    })
  );
};
