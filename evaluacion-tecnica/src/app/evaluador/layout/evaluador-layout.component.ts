import { Component, signal } from '@angular/core';
import { RouterOutlet, RouterLink, RouterLinkActive } from '@angular/router';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatListModule } from '@angular/material/list';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { AuthService } from '../../core/services/auth/auth.service';

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
    MatButtonModule
  ],
  templateUrl: './evaluador-layout.component.html',
  styleUrl: './evaluador-layout.component.scss',
})
export class EvaluadorLayoutComponent {
  sidenavAbierto = signal(true);

  constructor(public authService: AuthService) {}

  toggleSidenav(): void {
    this.sidenavAbierto.update(v => !v);
  }

  logout(): void {
    this.authService.logout();
  }
}
