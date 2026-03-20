import { Component, OnInit, signal, computed } from '@angular/core';
import { Router } from '@angular/router';
import { forkJoin, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { DatePipe } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatTableModule } from '@angular/material/table';
import { MatChipsModule } from '@angular/material/chips';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatDialog } from '@angular/material/dialog';
import { EvaluacionService } from '../../core/services/evaluacion/evaluacion.service';
import { CandidatoService } from '../../core/services/candidato/candidato.service';
import { NotificacionesService } from '../../core/services/notificaciones/notificaciones.service';
import { SesionService } from '../../core/services/sesion/sesion.service';
import { EvaluacionDto } from '../../core/models/evaluacion.model';
import { CandidatoDto } from '../../core/models/candidato.model';
import { LoadingSpinnerComponent } from '../../shared/components/loading-spinner/loading-spinner.component';
import { EmptyStateComponent } from '../../shared/components/empty-state/empty-state.component';
import { NivelBadgePipe } from '../../shared/pipes/nivel-badge-pipe';
import { RespuestasCrudasDialogComponent, RespuestasCrudasDialogData } from './respuestas-crudas-dialog.component';

export interface EvaluacionConCandidatos {
  evaluacion: EvaluacionDto;
  candidatos: CandidatoDto[];
  expandida: boolean;
}

@Component({
  selector: 'app-sesiones-lista',
  imports: [
    DatePipe,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatTableModule,
    MatChipsModule,
    MatTooltipModule,
    MatProgressSpinnerModule,
    LoadingSpinnerComponent,
    EmptyStateComponent,
    NivelBadgePipe
  ],
  templateUrl: './sesiones-lista.component.html',
  styleUrl: './sesiones-lista.component.scss',
})
export class SesionesListaComponent implements OnInit {
  items = signal<EvaluacionConCandidatos[]>([]);
  cargando = signal(true);
  columnasCandidatos = ['candidato', 'estado', 'fechas', 'foco', 'acciones'];

  totalRespondieron = computed(() =>
    this.items().reduce((acc, i) => acc + i.candidatos.filter(c => !!c.fechaFinRespuesta).length, 0)
  );
  totalPendientes = computed(() =>
    this.items().reduce((acc, i) => acc + i.candidatos.filter(c => !c.fechaFinRespuesta).length, 0)
  );
  totalSinAnalisis = computed(() =>
    this.items().reduce((acc, i) => acc + i.candidatos.filter(c => !!c.fechaFinRespuesta && !c.tieneResultado).length, 0)
  );

  creandoSesion = signal(false);

  constructor(
    private evaluacionService: EvaluacionService,
    private candidatoService: CandidatoService,
    private notificacionesService: NotificacionesService,
    private sesionService: SesionService,
    private router: Router,
    private dialog: MatDialog
  ) {}

  ngOnInit(): void {
    this.evaluacionService.listar().subscribe({
      next: evaluaciones => {
        const activas = evaluaciones.filter(e => e.estado === 2);
        if (activas.length === 0) {
          this.items.set([]);
          this.cargando.set(false);
          return;
        }
        const requests = activas.map(ev =>
          this.candidatoService.listar(ev.id).pipe(catchError(() => of([])))
        );
        forkJoin(requests).subscribe({
          next: candidatosPorEval => {
            const resultado = activas.map((ev, i) => ({
              evaluacion: ev,
              candidatos: candidatosPorEval[i] as CandidatoDto[],
              expandida: false
            }));
            this.items.set(resultado);

            // Build notification items from candidates who finished
            const notifItems = resultado.flatMap(r =>
              r.candidatos
                .filter(c => !!c.fechaFinRespuesta)
                .map(c => ({
                  candidatoNombre: c.nombre,
                  evaluacionTitulo: r.evaluacion.titulo,
                  evaluacionId: r.evaluacion.id,
                  candidatoId: c.id,
                  fecha: c.fechaFinRespuesta!,
                  tieneResultado: c.tieneResultado
                }))
            );
            this.notificacionesService.setNotificaciones(notifItems);
            this.cargando.set(false);
          },
          error: () => this.cargando.set(false)
        });
      },
      error: () => this.cargando.set(false)
    });
  }

  toggleExpandir(item: EvaluacionConCandidatos): void {
    item.expandida = !item.expandida;
  }

  estadoCandidato(c: CandidatoDto): 'sin-iniciar' | 'en-progreso' | 'respondio' | 'con-resultado' {
    if (c.tieneResultado) return 'con-resultado';
    if (c.fechaFinRespuesta) return 'respondio';
    if (c.fechaInicioRespuesta) return 'en-progreso';
    return 'sin-iniciar';
  }

  labelEstado(c: CandidatoDto): string {
    return ({ 'sin-iniciar': 'Sin iniciar', 'en-progreso': 'En progreso', 'respondio': 'Respondió', 'con-resultado': 'Con resultado' })[this.estadoCandidato(c)];
  }

  irASesion(ev: EvaluacionDto, c: CandidatoDto): void {
    this.creandoSesion.set(true);
    this.sesionService.crear({ evaluacionId: ev.id, candidatoId: c.id }).subscribe({
      next: sesion => {
        this.creandoSesion.set(false);
        this.router.navigate(['/evaluador/evaluacion', ev.id, 'sesion-vivo', sesion.sesionId]);
      },
      error: () => this.creandoSesion.set(false)
    });
  }

  verRespuestas(ev: EvaluacionDto, c: CandidatoDto): void {
    this.dialog.open(RespuestasCrudasDialogComponent, {
      data: { evaluacionId: ev.id, candidatoId: c.id, nombreCandidato: c.nombre } as RespuestasCrudasDialogData,
      width: '700px',
      maxHeight: '85vh'
    });
  }

  verResultado(ev: EvaluacionDto, c: CandidatoDto): void {
    this.router.navigate(['/evaluador/evaluacion', ev.id, 'resultado', c.id]);
  }

  irADetalle(evaluacionId: string): void {
    this.router.navigate(['/evaluador/evaluacion', evaluacionId]);
  }
}
