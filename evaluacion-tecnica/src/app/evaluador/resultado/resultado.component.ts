import { Component, OnInit, signal, computed } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { DatePipe } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatChipsModule } from '@angular/material/chips';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatSnackBar } from '@angular/material/snack-bar';
import { BaseChartDirective } from 'ng2-charts';
import { ChartConfiguration } from 'chart.js';
import { ScoreBadgeComponent } from '../../shared/components/score-badge/score-badge.component';
import { ResultadoService } from '../../core/services/resultado/resultado.service';
import { ReporteService } from '../../core/services/reporte/reporte.service';
import { ResultadoCompletoDto } from '../../core/models/resultado.model';
import { ResumenProctoringDto } from '../../core/models/proctoring.model';

@Component({
  selector: 'app-resultado',
  imports: [
    DatePipe,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatChipsModule,
    MatProgressSpinnerModule,
    MatTooltipModule,
    BaseChartDirective,
    ScoreBadgeComponent
  ],
  templateUrl: './resultado.component.html',
  styleUrl: './resultado.component.scss',
})
export class ResultadoComponent implements OnInit {
  resultado = signal<ResultadoCompletoDto | null>(null);
  proctoring = signal<ResumenProctoringDto | null>(null);
  cargando = signal(true);
  descargando = signal(false);

  private evaluacionId = '';
  private candidatoId = '';

  fortalezas = computed(() => {
    const r = this.resultado();
    if (!r?.fortalezasDetectadas) return [];
    return r.fortalezasDetectadas.split('\n').map(f => f.replace(/^[-•]\s*/, '').trim()).filter(f => f.length > 0);
  });

  brechas = computed(() => {
    const r = this.resultado();
    if (!r?.brechasDetectadas) return [];
    return r.brechasDetectadas.split('\n').map(b => b.replace(/^[-•]\s*/, '').trim()).filter(b => b.length > 0);
  });

  chartData = computed<ChartConfiguration<'bar'>['data']>(() => {
    const r = this.resultado();
    if (!r) return { labels: [], datasets: [] };

    const respuestas = [...r.respuestas].sort((a, b) => a.ordenEnEvaluacion - b.ordenEnEvaluacion);
    return {
      labels: respuestas.map(p => `P${p.ordenEnEvaluacion}`),
      datasets: [
        {
          label: 'Score obtenido',
          data: respuestas.map(p => p.scoreIA ?? 0),
          backgroundColor: respuestas.map(p => this.colorBarra(p.scoreIA ?? 0, p.puntajeMaximo)),
          borderRadius: 6
        },
        {
          label: 'Puntaje máximo',
          data: respuestas.map(p => p.puntajeMaximo),
          backgroundColor: 'rgba(203, 213, 225, 0.4)',
          borderRadius: 6
        }
      ]
    };
  });

  chartOptions: ChartConfiguration<'bar'>['options'] = {
    responsive: true,
    plugins: {
      legend: { position: 'bottom' }
    },
    scales: {
      y: { beginAtZero: true }
    }
  };

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private resultadoService: ResultadoService,
    private reporteService: ReporteService,
    private snackBar: MatSnackBar
  ) {}

  ngOnInit(): void {
    this.evaluacionId = this.route.snapshot.paramMap.get('eId')!;
    this.candidatoId = this.route.snapshot.paramMap.get('cId')!;

    this.resultadoService.obtenerResultado(this.candidatoId).subscribe({
      next: r => {
        this.resultado.set(r);
        this.cargando.set(false);
      },
      error: () => {
        this.snackBar.open('Error al cargar el resultado', 'OK', { duration: 3000 });
        this.cargando.set(false);
      }
    });

    this.resultadoService.obtenerProctoring(this.candidatoId).subscribe({
      next: p => this.proctoring.set(p),
      error: () => {} // silent - proctoring is optional
    });
  }

  colorScore(score: number): string {
    if (score >= 80) return 'text-green-600';
    if (score >= 50) return 'text-amber-600';
    return 'text-red-600';
  }

  colorScorePregunta(score: number | undefined, maximo: number): string {
    if (score === undefined || score === null) return 'text-gray-400';
    const porcentaje = (score / maximo) * 100;
    if (porcentaje >= 80) return 'text-green-600';
    if (porcentaje >= 50) return 'text-amber-600';
    return 'text-red-600';
  }

  descargarPdf(): void {
    this.descargando.set(true);
    this.reporteService.descargarPdfCandidato(this.candidatoId).subscribe({
      next: blob => {
        const url = URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = `reporte-${this.resultado()?.candidatoNombre?.replace(/\s/g, '_') ?? 'candidato'}.pdf`;
        a.click();
        URL.revokeObjectURL(url);
        this.descargando.set(false);
      },
      error: () => {
        this.snackBar.open('Error al descargar PDF', 'OK', { duration: 3000 });
        this.descargando.set(false);
      }
    });
  }

  volver(): void {
    this.router.navigate(['/evaluador/evaluacion', this.evaluacionId]);
  }

  private colorBarra(score: number, maximo: number): string {
    const porcentaje = (score / maximo) * 100;
    if (porcentaje >= 80) return 'rgba(16, 185, 129, 0.7)';
    if (porcentaje >= 50) return 'rgba(245, 158, 11, 0.7)';
    return 'rgba(239, 68, 68, 0.7)';
  }
}
