import { Component, OnInit, signal } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatDialog } from '@angular/material/dialog';
import { AuthService } from '../../core/services/auth/auth.service';
import { PruebaService } from '../../core/services/prueba/prueba.service';
import { EvaluacionCandidatoDto } from '../../core/models/candidato.model';
import { NivelTecnico } from '../../core/models/evaluacion.model';
import { ConfirmDialogComponent, ConfirmDialogData } from '../../shared/components/confirm-dialog/confirm-dialog.component';

@Component({
  selector: 'app-instrucciones',
  imports: [
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatProgressSpinnerModule
  ],
  templateUrl: './instrucciones.component.html',
  styleUrl: './instrucciones.component.scss',
})
export class InstruccionesComponent implements OnInit {
  evaluacion = signal<EvaluacionCandidatoDto | null>(null);
  cargando = signal(true);
  iniciando = signal(false);
  error = signal<string | null>(null);

  estaAutenticado = false;

  private token = '';

  constructor(
    public route: ActivatedRoute,
    public router: Router,
    private pruebaService: PruebaService,
    private snackBar: MatSnackBar,
    private authService: AuthService,
    private dialog: MatDialog
  ) {
    this.estaAutenticado = this.authService.isAuthenticated();
  }

  ngOnInit(): void {
    this.token = this.route.snapshot.paramMap.get('token')
      ?? this.route.parent?.snapshot.paramMap.get('token') ?? '';
    this.pruebaService.obtenerPorToken(this.token).subscribe({
      next: ev => {
        this.evaluacion.set(ev);
        this.cargando.set(false);
      },
      error: () => {
        this.error.set('Token inválido o evaluación no encontrada.');
        this.cargando.set(false);
      }
    });
  }

  nombreNivel(nivel: number): string {
    const map: Record<number, string> = { 0: 'Junior', 1: 'Junior', 2: 'Mid', 3: 'Senior', 4: 'Lead' };
    return map[nivel] ?? 'Junior';
  }

  verResultado(): void {
    if (this.estaAutenticado) {
      this.router.navigate(['/panel-candidato/evaluacion', this.token, 'resultado']);
    } else {
      this.router.navigate(['/candidato', this.token, 'resultado']);
    }
  }

  iniciarPrueba(): void {
    const ev = this.evaluacion();
    this.dialog.open(ConfirmDialogComponent, {
      data: {
        titulo: 'Iniciar Evaluación',
        mensaje: `¿Estás listo para iniciar? Tendrás ${ev?.tiempoLimiteTotalMinutos ?? 0} minutos para completar la prueba. Una vez iniciada, no podrás pausarla.`,
        textoConfirmar: 'Iniciar Prueba',
        textoCancelar: 'Aún no'
      } as ConfirmDialogData,
      width: '440px'
    }).afterClosed().subscribe(result => {
      if (!result) return;
      this.iniciando.set(true);
      this.pruebaService.iniciar(this.token).subscribe({
        next: () => {
          this.router.navigate(['/candidato', this.token, 'responder']);
        },
        error: (err) => {
          this.iniciando.set(false);
          const msg = err?.error?.message ?? 'Error al iniciar la prueba';
          this.snackBar.open(msg, 'OK', { duration: 4000 });
        }
      });
    });
  }
}
