import { Component, OnInit, signal, computed } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatTableModule } from '@angular/material/table';
import { MatSnackBar } from '@angular/material/snack-bar';
import { BaseChartDirective } from 'ng2-charts';
import { ChartData, ChartOptions } from 'chart.js';
import { ResultadoService } from '../../core/services/resultado/resultado.service';
import { ComparacionCandidatosDto, CandidatoComparadoDto } from '../../core/models/resultado.model';
import { ScoreBadgeComponent } from '../../shared/components/score-badge/score-badge.component';

@Component({
  selector: 'app-comparar',
  imports: [
    MatCardModule, MatButtonModule, MatIconModule, MatProgressSpinnerModule,
    MatTableModule, BaseChartDirective, ScoreBadgeComponent
  ],
  templateUrl: './comparar.component.html',
  styleUrl: './comparar.component.scss',
})
export class CompararComponent implements OnInit {
  comparacion = signal<ComparacionCandidatosDto | null>(null);
  cargando = signal(true);

  private evaluacionId = '';
  private coloresGrafica = ['#3b82f6', '#10b981', '#f59e0b', '#ef4444', '#8b5cf6', '#06b6d4'];

  radarData = computed<ChartData<'radar'>>(() => {
    const c = this.comparacion();
    if (!c || c.candidatos.length === 0) return { labels: [], datasets: [] };

    const labels = c.candidatos[0].scoresPorPregunta.map(s => `P${s.ordenEnEvaluacion}`);
    const datasets = c.candidatos.map((cand, i) => ({
      label: cand.nombre,
      data: cand.scoresPorPregunta.map(s => {
        const max = s.puntajeMaximo || 1;
        return ((s.scoreIA ?? 0) / max) * 100;
      }),
      borderColor: this.coloresGrafica[i % this.coloresGrafica.length],
      backgroundColor: this.coloresGrafica[i % this.coloresGrafica.length] + '20',
      pointBackgroundColor: this.coloresGrafica[i % this.coloresGrafica.length],
    }));

    return { labels, datasets };
  });

  radarOptions: ChartOptions<'radar'> = {
    responsive: true,
    plugins: { legend: { position: 'bottom' } },
    scales: { r: { beginAtZero: true, max: 100, ticks: { stepSize: 20 } } }
  };

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private resultadoService: ResultadoService,
    private snackBar: MatSnackBar
  ) {}

  ngOnInit(): void {
    this.evaluacionId = this.route.snapshot.paramMap.get('eId') ?? '';
    const idsParam = this.route.snapshot.queryParamMap.get('ids') ?? '';
    const ids = idsParam.split(',').filter(Boolean);

    if (ids.length < 2) {
      this.snackBar.open('Selecciona al menos 2 candidatos para comparar', 'OK', { duration: 3000 });
      this.router.navigate(['/evaluador/evaluacion', this.evaluacionId, 'ranking']);
      return;
    }

    this.resultadoService.compararCandidatos(this.evaluacionId, ids).subscribe({
      next: data => {
        this.comparacion.set(data);
        this.cargando.set(false);
      },
      error: () => {
        this.snackBar.open('Error al cargar comparación', 'OK', { duration: 3000 });
        this.cargando.set(false);
      }
    });
  }

  volver(): void {
    this.router.navigate(['/evaluador/evaluacion', this.evaluacionId, 'ranking']);
  }

  colorScore(score: number): string {
    if (score >= 80) return 'text-green-600';
    if (score >= 50) return 'text-amber-600';
    return 'text-red-600';
  }

  mejorScore(preguntaIndex: number): number {
    const c = this.comparacion();
    if (!c) return 0;
    return Math.max(...c.candidatos.map(cand => cand.scoresPorPregunta[preguntaIndex]?.scoreIA ?? 0));
  }
}
