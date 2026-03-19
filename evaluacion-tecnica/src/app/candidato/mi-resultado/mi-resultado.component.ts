import { Component, OnInit, signal } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { DatePipe } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatChipsModule } from '@angular/material/chips';
import { PruebaService } from '../../core/services/prueba/prueba.service';
import { ResultadoCandidatoPublicoDto } from '../../core/models/resultado.model';

@Component({
  selector: 'app-mi-resultado',
  imports: [
    DatePipe,
    MatCardModule,
    MatIconModule,
    MatProgressSpinnerModule,
    MatChipsModule
  ],
  templateUrl: './mi-resultado.component.html',
  styleUrl: './mi-resultado.component.scss',
})
export class MiResultadoComponent implements OnInit {
  resultado = signal<ResultadoCandidatoPublicoDto | null>(null);
  cargando = signal(true);
  sinResultado = signal(false);

  private token = '';

  constructor(
    private route: ActivatedRoute,
    private pruebaService: PruebaService
  ) {}

  ngOnInit(): void {
    this.token = this.route.parent!.snapshot.paramMap.get('token')!;
    this.pruebaService.obtenerResultado(this.token).subscribe({
      next: res => {
        this.resultado.set(res);
        this.cargando.set(false);
      },
      error: () => {
        this.sinResultado.set(true);
        this.cargando.set(false);
      }
    });
  }

  colorScore(score: number): string {
    if (score >= 80) return 'text-green-600';
    if (score >= 60) return 'text-blue-600';
    if (score >= 40) return 'text-amber-600';
    return 'text-red-600';
  }

  bgScore(score: number): string {
    if (score >= 80) return 'bg-green-50';
    if (score >= 60) return 'bg-blue-50';
    if (score >= 40) return 'bg-amber-50';
    return 'bg-red-50';
  }
}
