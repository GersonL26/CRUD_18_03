import { Component, OnInit, signal } from '@angular/core';
import { Router } from '@angular/router';
import { DatePipe } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { CandidatoService } from '../../core/services/candidato/candidato.service';
import { EvaluacionAsignadaDto } from '../../core/models/candidato.model';
import { NivelTecnico } from '../../core/models/evaluacion.model';

@Component({
  selector: 'app-mis-evaluaciones',
  imports: [
    DatePipe,
    MatCardModule,
    MatIconModule,
    MatButtonModule,
    MatProgressSpinnerModule
  ],
  templateUrl: './mis-evaluaciones.component.html',
  styleUrl: './mis-evaluaciones.component.scss',
})
export class MisEvaluacionesComponent implements OnInit {
  evaluaciones = signal<EvaluacionAsignadaDto[]>([]);
  cargando = signal(true);

  constructor(
    private candidatoService: CandidatoService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.candidatoService.misEvaluaciones().subscribe({
      next: data => {
        this.evaluaciones.set(data);
        this.cargando.set(false);
      },
      error: () => this.cargando.set(false)
    });
  }

  nombreNivel(nivel: number): string {
    const map: Record<number, string> = { 0: 'Junior', 1: 'Junior', 2: 'Mid', 3: 'Senior', 4: 'Lead' };
    return map[nivel] ?? 'Junior';
  }

  iniciar(token: string): void {
    this.router.navigate(['/candidato', token]);
  }

  verResultado(token: string): void {
    this.router.navigate(['/candidato', token, 'resultado']);
  }
}
