import { Component, signal, computed, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatTableModule } from '@angular/material/table';
import { MatChipsModule } from '@angular/material/chips';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { EvaluacionService } from '../../core/services/evaluacion/evaluacion.service';
import { EvaluacionDto } from '../../core/models/evaluacion.model';
import { LoadingSpinnerComponent } from '../../shared/components/loading-spinner/loading-spinner.component';
import { EmptyStateComponent } from '../../shared/components/empty-state/empty-state.component';
import { NivelBadgePipe } from '../../shared/pipes/nivel-badge-pipe';

@Component({
  selector: 'app-rankings-lista',
  imports: [
    FormsModule,
    MatCardModule, MatButtonModule, MatIconModule, MatTableModule,
    MatChipsModule, MatTooltipModule, MatFormFieldModule, MatInputModule,
    MatSelectModule,
    LoadingSpinnerComponent, EmptyStateComponent, NivelBadgePipe
  ],
  templateUrl: './rankings-lista.component.html',
  styleUrl: './rankings-lista.component.scss',
})
export class RankingsListaComponent implements OnInit {
  evaluaciones = signal<EvaluacionDto[]>([]);
  cargando = signal(true);

  filtroTexto = signal('');
  filtroTecnologia = signal('');
  filtroNivel = signal(0);

  columnas = ['titulo', 'tecnologia', 'nivel', 'estado', 'acciones'];

  tecnologias = computed(() => {
    const set = new Set(this.evaluaciones().map(e => e.tecnologia));
    return Array.from(set).sort();
  });

  evaluacionesFiltradas = computed(() => {
    let lista = this.evaluaciones();
    const txt = this.filtroTexto().toLowerCase();
    const tec = this.filtroTecnologia();
    const niv = this.filtroNivel();

    if (txt) lista = lista.filter(e => e.titulo.toLowerCase().includes(txt) || e.tecnologia.toLowerCase().includes(txt));
    if (tec) lista = lista.filter(e => e.tecnologia === tec);
    if (niv) lista = lista.filter(e => e.nivel === niv);
    return lista;
  });

  constructor(
    private evaluacionService: EvaluacionService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.evaluacionService.listar().subscribe({
      next: (data) => {
        this.evaluaciones.set(data.filter(e => e.estado !== 1));
        this.cargando.set(false);
      },
      error: () => this.cargando.set(false)
    });
  }

  verRanking(evaluacionId: string): void {
    this.router.navigate(['/evaluador/evaluacion', evaluacionId, 'ranking']);
  }

  estadoLabel(estado: number): string {
    const labels: Record<number, string> = { 1: 'Borrador', 2: 'Activa', 3: 'Cerrada' };
    return labels[estado] ?? '—';
  }

  contarPorEstado(estado: number): number {
    return this.evaluaciones().filter(e => e.estado === estado).length;
  }

  limpiarFiltros(): void {
    this.filtroTexto.set(''); this.filtroTecnologia.set(''); this.filtroNivel.set(0);
  }
}
