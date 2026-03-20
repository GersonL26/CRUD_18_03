import { Component } from '@angular/core';
import { RouterOutlet, RouterLink, RouterLinkActive } from '@angular/router';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatListModule } from '@angular/material/list';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatDialog } from '@angular/material/dialog';
import { MatTooltipModule } from '@angular/material/tooltip';
import { AuthService } from '../../core/services/auth/auth.service';
import { ConfirmDialogComponent, ConfirmDialogData } from '../../shared/components/confirm-dialog/confirm-dialog.component';

@Component({
  selector: 'app-candidato-layout',
  imports: [
    RouterOutlet,
    RouterLink,
    RouterLinkActive,
    MatToolbarModule,
    MatIconModule,
    MatButtonModule,
    MatListModule,
    MatSidenavModule,
    MatTooltipModule
  ],
  templateUrl: './candidato-layout.component.html',
  styleUrl: './candidato-layout.component.scss',
})
export class CandidatoLayoutComponent {
  constructor(
    public authService: AuthService,
    private dialog: MatDialog
  ) {}

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
