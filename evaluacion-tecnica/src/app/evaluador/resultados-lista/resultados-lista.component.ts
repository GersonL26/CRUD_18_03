import { Component, signal, computed, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { DatePipe } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatTableModule } from '@angular/material/table';
import { MatChipsModule } from '@angular/material/chips';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { EvaluacionService } from '../../core/services/evaluacion/evaluacion.service';
import { ResultadoService } from '../../core/services/resultado/resultado.service';
import { ReporteService } from '../../core/services/reporte/reporte.service';
import { EvaluacionDto } from '../../core/models/evaluacion.model';
import { ResumenEvaluacionResultadosDto, ResumenCandidatoDto } from '../../core/models/resultado.model';
import { LoadingSpinnerComponent } from '../../shared/components/loading-spinner/loading-spinner.component';
import { EmptyStateComponent } from '../../shared/components/empty-state/empty-state.component';
import { ScoreBadgeComponent } from '../../shared/components/score-badge/score-badge.component';
import { forkJoin } from 'rxjs';

interface FilaResultado {
  evaluacionId: string;
  tituloEvaluacion: string;
  tecnologia: string;
  nivel: string;
  candidatoId: string;
  nombre: string;
  email: string;
  estado: string;
  scoreTotal?: number;
  recomendacion?: string;
  tiempoInvertido?: string;
  fechaAnalisis?: string;
  resultadoLiberado: boolean;
}

@Component({
  selector: 'app-resultados-lista',
  imports: [
    FormsModule, DatePipe,
    MatCardModule, MatButtonModule, MatIconModule, MatTableModule,
    MatChipsModule, MatTooltipModule, MatFormFieldModule, MatInputModule,
    MatSelectModule, MatProgressSpinnerModule,
    LoadingSpinnerComponent, EmptyStateComponent, ScoreBadgeComponent
  ],
  templateUrl: './resultados-lista.component.html',
  styleUrl: './resultados-lista.component.scss',
})
export class ResultadosListaComponent implements OnInit {
  evaluaciones = signal<EvaluacionDto[]>([]);
  filas = signal<FilaResultado[]>([]);
  cargando = signal(true);

  filtroTexto = signal('');
  filtroEvaluacion = signal('');
  filtroEstado = signal('');
  filtroFechaDesde = signal('');
  filtroFechaHasta = signal('');

  columnas = ['nombre', 'email', 'evaluacion', 'score', 'recomendacion', 'estado', 'fecha', 'acciones'];

  filasFiltradas = computed(() => {
    let rows = this.filas();
    const txt = this.filtroTexto().toLowerCase();
    const evId = this.filtroEvaluacion();
    const est = this.filtroEstado();
    const desde = this.filtroFechaDesde();
    const hasta = this.filtroFechaHasta();

    if (txt) rows = rows.filter(r =>
      r.nombre.toLowerCase().includes(txt) || r.email.toLowerCase().includes(txt));
    if (evId) rows = rows.filter(r => r.evaluacionId === evId);
    if (est) rows = rows.filter(r => r.estado === est);
    if (desde) rows = rows.filter(r => r.fechaAnalisis && r.fechaAnalisis >= desde);
    if (hasta) rows = rows.filter(r => r.fechaAnalisis && r.fechaAnalisis <= hasta + 'T23:59:59');
    return rows;
  });

  totalCandidatos = computed(() => this.filas().length);
  totalAnalizados = computed(() => this.filas().filter(f => f.scoreTotal != null).length);
  scorePromedio = computed(() => {
    const analizados = this.filas().filter(f => f.scoreTotal != null);
    if (!analizados.length) return 0;
    return Math.round(analizados.reduce((s, f) => s + (f.scoreTotal ?? 0), 0) / analizados.length);
  });

  constructor(
    private evaluacionService: EvaluacionService,
    private resultadoService: ResultadoService,
    private reporteService: ReporteService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.evaluacionService.listar().subscribe({
      next: (evals) => {
        const activas = evals.filter(e => e.estado !== 1);
        this.evaluaciones.set(activas);
        if (!activas.length) { this.cargando.set(false); return; }

        const requests = activas.map(ev => this.resultadoService.obtenerResumenEvaluacion(ev.id));
        forkJoin(requests).subscribe({
          next: resumenes => {
            const result: FilaResultado[] = [];
            resumenes.forEach(resumen => {
              resumen.candidatos.forEach(c => {
                result.push({
                  evaluacionId: resumen.evaluacionId,
                  tituloEvaluacion: resumen.titulo,
                  tecnologia: resumen.tecnologia,
                  nivel: resumen.nivel,
                  candidatoId: c.candidatoId,
                  nombre: c.nombre, email: c.email,
                  estado: c.estado,
                  scoreTotal: c.scoreTotal,
                  recomendacion: c.recomendacion,
                  tiempoInvertido: c.tiempoInvertido,
                  fechaAnalisis: c.fechaAnalisis,
                  resultadoLiberado: c.resultadoLiberado
                });
              });
            });
            this.filas.set(result);
            this.cargando.set(false);
          },
          error: () => this.cargando.set(false)
        });
      },
      error: () => this.cargando.set(false)
    });
  }

  verResultado(fila: FilaResultado): void {
    this.router.navigate(['/evaluador/evaluacion', fila.evaluacionId, 'resultado', fila.candidatoId]);
  }

  descargarPdf(fila: FilaResultado): void {
    this.reporteService.descargarPdfCandidato(fila.candidatoId).subscribe({
      next: blob => {
        const url = URL.createObjectURL(blob);
        const a = document.createElement('a'); a.href = url;
        a.download = `resultado-${fila.nombre.replace(/\s/g, '_')}.pdf`;
        a.click(); URL.revokeObjectURL(url);
      }
    });
  }

  limpiarFiltros(): void {
    this.filtroTexto.set(''); this.filtroEvaluacion.set('');
    this.filtroEstado.set(''); this.filtroFechaDesde.set('');
    this.filtroFechaHasta.set('');
  }

  estadoLabel(estado: string): string {
    const map: Record<string, string> = {
      'Pendiente': 'Pendiente', 'Respondido': 'Respondido', 'Analizado': 'Analizado'
    };
    return map[estado] ?? estado;
  }

  estadoClase(estado: string): string {
    if (estado === 'Analizado') return 'estado-analizado';
    if (estado === 'Respondido') return 'estado-respondido';
    return 'estado-pendiente';
  }
}
