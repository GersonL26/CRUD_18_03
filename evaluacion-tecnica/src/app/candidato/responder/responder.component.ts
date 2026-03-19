import { Component, OnInit, OnDestroy, signal, computed } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatSnackBar } from '@angular/material/snack-bar';
import { FormsModule } from '@angular/forms';
import { PruebaService } from '../../core/services/prueba/prueba.service';
import { EvaluacionCandidatoDto, PreguntaCandidatoDto, RespuestaItemDto } from '../../core/models/candidato.model';
import { TipoPregunta } from '../../core/models/evaluacion.model';

@Component({
  selector: 'app-responder',
  imports: [
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatProgressSpinnerModule,
    MatProgressBarModule,
    FormsModule
  ],
  templateUrl: './responder.component.html',
  styleUrl: './responder.component.scss',
})
export class ResponderComponent implements OnInit, OnDestroy {
  evaluacion = signal<EvaluacionCandidatoDto | null>(null);
  cargando = signal(true);
  enviando = signal(false);
  error = signal<string | null>(null);

  preguntaActualIndex = signal(0);
  respuestas = signal<Map<string, RespuestaItemDto>>(new Map());
  tiempoRestante = signal(0);

  private token = '';
  private intervalo: ReturnType<typeof setInterval> | null = null;
  private iniciosPregunta: Map<string, number> = new Map();

  preguntaActual = computed(() => {
    const ev = this.evaluacion();
    if (!ev) return null;
    const sorted = [...ev.preguntas].sort((a, b) => a.ordenEnEvaluacion - b.ordenEnEvaluacion);
    return sorted[this.preguntaActualIndex()] ?? null;
  });

  totalPreguntas = computed(() => this.evaluacion()?.preguntas.length ?? 0);

  progreso = computed(() => {
    const total = this.totalPreguntas();
    if (!total) return 0;
    return ((this.preguntaActualIndex() + 1) / total) * 100;
  });

  tiempoFormateado = computed(() => {
    const s = this.tiempoRestante();
    const m = Math.floor(s / 60);
    const sec = s % 60;
    return `${m.toString().padStart(2, '0')}:${sec.toString().padStart(2, '0')}`;
  });

  esUltima = computed(() => this.preguntaActualIndex() === this.totalPreguntas() - 1);

  TipoPregunta = TipoPregunta;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private pruebaService: PruebaService,
    private snackBar: MatSnackBar
  ) {}

  ngOnInit(): void {
    this.token = this.route.parent!.snapshot.paramMap.get('token')!;
    this.pruebaService.obtenerPorToken(this.token).subscribe({
      next: ev => {
        if (ev.yaRespondio) {
          this.router.navigate(['resultado'], { relativeTo: this.route.parent });
          return;
        }
        this.evaluacion.set(ev);
        this.cargando.set(false);
        this.tiempoRestante.set(ev.tiempoLimiteTotalMinutos * 60);
        this.iniciarTemporizador();
        this.registrarInicioPregunta();
      },
      error: () => {
        this.error.set('No se pudo cargar la evaluación.');
        this.cargando.set(false);
      }
    });
  }

  ngOnDestroy(): void {
    this.detenerTemporizador();
  }

  getRespuestaContenido(preguntaId: string): string {
    return this.respuestas().get(preguntaId)?.contenido ?? '';
  }

  setRespuestaContenido(preguntaId: string, contenido: string): void {
    const map = new Map(this.respuestas());
    const existing = map.get(preguntaId);
    map.set(preguntaId, { preguntaId, contenido, tiempoUsadoSegundos: existing?.tiempoUsadoSegundos ?? 0 });
    this.respuestas.set(map);
  }

  anterior(): void {
    this.guardarTiempoPregunta();
    this.preguntaActualIndex.update(i => Math.max(0, i - 1));
    this.registrarInicioPregunta();
  }

  siguiente(): void {
    this.guardarTiempoPregunta();
    this.preguntaActualIndex.update(i => Math.min(this.totalPreguntas() - 1, i + 1));
    this.registrarInicioPregunta();
  }

  irAPregunta(index: number): void {
    this.guardarTiempoPregunta();
    this.preguntaActualIndex.set(index);
    this.registrarInicioPregunta();
  }

  enviarRespuestas(): void {
    if (this.enviando()) return;
    this.guardarTiempoPregunta();
    this.enviando.set(true);
    this.detenerTemporizador();

    const respuestasArray = Array.from(this.respuestas().values());
    const ev = this.evaluacion()!;
    const todasIds = ev.preguntas.map(p => p.id);
    const respondidas = new Set(respuestasArray.map(r => r.preguntaId));
    for (const id of todasIds) {
      if (!respondidas.has(id)) {
        respuestasArray.push({ preguntaId: id, contenido: '', tiempoUsadoSegundos: 0 });
      }
    }

    this.pruebaService.enviarRespuestas(this.token, { respuestas: respuestasArray }).subscribe({
      next: () => {
        this.snackBar.open('Respuestas enviadas correctamente', 'OK', { duration: 3000 });
        this.router.navigate(['resultado'], { relativeTo: this.route.parent });
      },
      error: (err) => {
        this.enviando.set(false);
        this.iniciarTemporizador();
        const msg = err?.error?.message ?? 'Error al enviar respuestas';
        this.snackBar.open(msg, 'OK', { duration: 4000 });
      }
    });
  }

  preguntaRespondida(preguntaId: string): boolean {
    const r = this.respuestas().get(preguntaId);
    return !!r && r.contenido.trim().length > 0;
  }

  preguntasOrdenadas(): PreguntaCandidatoDto[] {
    const ev = this.evaluacion();
    if (!ev) return [];
    return [...ev.preguntas].sort((a, b) => a.ordenEnEvaluacion - b.ordenEnEvaluacion);
  }

  private iniciarTemporizador(): void {
    this.detenerTemporizador();
    this.intervalo = setInterval(() => {
      this.tiempoRestante.update(t => {
        if (t <= 1) {
          this.enviarRespuestas();
          return 0;
        }
        return t - 1;
      });
    }, 1000);
  }

  private detenerTemporizador(): void {
    if (this.intervalo) {
      clearInterval(this.intervalo);
      this.intervalo = null;
    }
  }

  private registrarInicioPregunta(): void {
    const p = this.preguntaActual();
    if (p) this.iniciosPregunta.set(p.id, Date.now());
  }

  private guardarTiempoPregunta(): void {
    const p = this.preguntaActual();
    if (!p) return;
    const inicio = this.iniciosPregunta.get(p.id);
    if (!inicio) return;
    const elapsed = Math.round((Date.now() - inicio) / 1000);
    const map = new Map(this.respuestas());
    const existing = map.get(p.id);
    const prev = existing?.tiempoUsadoSegundos ?? 0;
    map.set(p.id, {
      preguntaId: p.id,
      contenido: existing?.contenido ?? '',
      tiempoUsadoSegundos: prev + elapsed
    });
    this.respuestas.set(map);
    this.iniciosPregunta.delete(p.id);
  }
}
