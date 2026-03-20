import { Component, signal } from '@angular/core';
import { RouterOutlet, RouterLink, RouterLinkActive } from '@angular/router';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatListModule } from '@angular/material/list';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatDialog } from '@angular/material/dialog';
import { AuthService } from '../../core/services/auth/auth.service';
import { ConfirmDialogComponent, ConfirmDialogData } from '../../shared/components/confirm-dialog/confirm-dialog.component';

@Component({
  selector: 'app-evaluador-layout',
  imports: [
    RouterOutlet,
    RouterLink,
    RouterLinkActive,
    MatToolbarModule,
    MatSidenavModule,
    MatListModule,
    MatIconModule,
    MatButtonModule,
    MatTooltipModule
  ],
  templateUrl: './evaluador-layout.component.html',
  styleUrl: './evaluador-layout.component.scss',
})
export class EvaluadorLayoutComponent {
  sidenavAbierto = signal(true);

  constructor(
    public authService: AuthService,
    private dialog: MatDialog
  ) {}

  toggleSidenav(): void {
    this.sidenavAbierto.update(v => !v);
  }

  logout(): void {
    this.dialog.open(ConfirmDialogComponent, {
      data: {
        titulo: 'Cerrar sesión',
        mensaje: '¿Estás seguro de que deseas cerrar sesión?',
        textoConfirmar: 'Cerrar sesión',
        color: 'warn'
      } as ConfirmDialogData,
      width: '400px'
    }).afterClosed().subscribe(result => {
      if (result) this.authService.logout();
    });
  }
}
