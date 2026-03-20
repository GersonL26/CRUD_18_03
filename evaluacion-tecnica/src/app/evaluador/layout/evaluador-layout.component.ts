import { Component, OnInit, signal, Signal } from '@angular/core';
import { RouterOutlet, RouterLink, RouterLinkActive, Router } from '@angular/router';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatListModule } from '@angular/material/list';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatBadgeModule } from '@angular/material/badge';
import { MatMenuModule } from '@angular/material/menu';
import { MatDialog } from '@angular/material/dialog';
import { DatePipe } from '@angular/common';
import { AuthService } from '../../core/services/auth/auth.service';
import { NotificacionesService, NotificacionItem } from '../../core/services/notificaciones/notificaciones.service';
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
    MatTooltipModule,
    MatBadgeModule,
    MatMenuModule,
    DatePipe
  ],
  templateUrl: './evaluador-layout.component.html',
  styleUrl: './evaluador-layout.component.scss',
})
export class EvaluadorLayoutComponent implements OnInit {
  sidenavAbierto = signal(true);
  notifCount!: Signal<number>;
  notifItems!: Signal<NotificacionItem[]>;

  constructor(
    public authService: AuthService,
    private notificacionesService: NotificacionesService,
    private router: Router,
    private dialog: MatDialog
  ) {
    this.notifCount = notificacionesService.count;
    this.notifItems = notificacionesService.items;
  }

  ngOnInit(): void {
    this.notificacionesService.cargar();
  }

  toggleSidenav(): void {
    this.sidenavAbierto.update(v => !v);
  }

  verResultado(item: NotificacionItem): void {
    if (item.tieneResultado) {
      this.router.navigate(['/evaluador/resultado', item.candidatoId]);
    } else {
      this.router.navigate(['/evaluador/evaluacion', item.evaluacionId], { fragment: 'candidatos' });
    }
  }

  irASesiones(): void {
    this.router.navigate(['/evaluador/sesiones']);
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
