import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';

export const evaluadorGuard: CanActivateFn = () => {
  const router = inject(Router);
  const token = localStorage.getItem('jwt_token');

  if (!token) {
    router.navigate(['/login']);
    return false;
  }

  try {
    const payload = JSON.parse(atob(token.split('.')[1]));
    const rol = payload['http://schemas.microsoft.com/ws/2008/06/identity/claims/role']
      || payload['role'];

    if (rol === 'Evaluador' || rol === 'Admin') {
      return true;
    }
  } catch {
    localStorage.removeItem('jwt_token');
  }

  router.navigate(['/login']);
  return false;
};
