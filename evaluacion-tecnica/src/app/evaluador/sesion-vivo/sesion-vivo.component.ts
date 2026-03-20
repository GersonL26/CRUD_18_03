import { Component, OnInit, OnDestroy, signal, computed, inject } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatChipsModule } from '@angular/material/chips';
import { MatTooltipModule } from '@angular/material/tooltip';
import { SesionService } from '../../core/services/sesion/sesion.service';
import { ResultadoService } from '../../core/services/resultado/resultado.service';
import { AudioTranscripcionService } from '../../core/services/audio/audio-transcripcion.service';
import { EstadoSesionDto, PreguntaEnVivoDto } from '../../core/models/sesion.model';

interface HistorialPregunta {
  index: number;
  texto: string;
  respondida: boolean;
  transcripcion?: string;
}

@Component({
  selector: 'app-sesion-vivo',
  imports: [
    FormsModule,
    MatCardModule, MatButtonModule, MatIconModule,
    MatProgressSpinnerModule, MatProgressBarModule, MatChipsModule, MatTooltipModule
  ],
  templateUrl: './sesion-vivo.component.html',
  styleUrl: './sesion-vivo.component.scss',
})
export class SesionVivoComponent implements OnInit, OnDestroy {
  estado = signal<EstadoSesionDto | null>(null);
  preguntaActual = signal<PreguntaEnVivoDto | null>(null);
  cargando = signal(true);
  sesionIniciada = signal(false);
  sesionCompletada = signal(false);
  sesionPausada = signal(false);
  cancelando = signal(false);
  historial = signal<HistorialPregunta[]>([]);
  segundosTimer = signal(0);

  // Recording & transcription
  transcripcionTexto = signal('');
  transcribiendoWhisper = signal(false);
  enviandoRespuesta = signal(false);
  ultimoAudioBlob = signal<Blob | null>(null);

  // Analysis state
  analizando = signal(false);
  analisisCompletado = signal(false);
  resultadoCandidatoId = signal<string | null>(null);

  progreso = computed(() => {
    const p = this.preguntaActual();
    if (!p) return 0;
    return Math.round(((p.index + 1) / p.totalPreguntas) * 100);
  });

  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private sesionService = inject(SesionService);
  private resultadoService = inject(ResultadoService);
  private audioService = inject(AudioTranscripcionService);
  private snackBar = inject(MatSnackBar);

  grabando = this.audioService.grabando;
  nivelAudio = this.audioService.nivelAudio;
  transcripcionParcial = this.audioService.transcripcionParcial;

  private sesionId = '';
  private evaluacionId = '';
  private timerInterval: any;

  ngOnInit(): void {
    this.evaluacionId = this.route.snapshot.paramMap.get('eId') ?? '';
    this.sesionId = this.route.snapshot.paramMap.get('sId') ?? '';

    this.sesionService.obtenerEstado(this.sesionId).subscribe({
      next: est => {
        this.estado.set(est);
        if (est.fueCompletada) {
          this.sesionCompletada.set(true);
        } else if (est.sesionActiva) {
          this.sesionIniciada.set(true);
          this.segundosTimer.set(est.segundosTranscurridos);
          this.iniciarTimer();
          this.cargarPreguntaActual();
        }
        this.cargando.set(false);
      },
      error: () => {
        this.snackBar.open('Error al cargar sesión', 'OK', { duration: 3000 });
        this.cargando.set(false);
      }
    });
  }

  ngOnDestroy(): void {
    clearInterval(this.timerInterval);
    if (this.grabando()) {
      this.audioService.detenerGrabacion();
    }
  }

  iniciarEntrevista(): void {
    this.cargando.set(true);
    this.sesionService.iniciarPorEvaluador(this.sesionId).subscribe({
      next: pregunta => {
        this.sesionIniciada.set(true);
        this.preguntaActual.set(pregunta);
        this.agregarHistorial(pregunta);
        this.segundosTimer.set(0);
        this.iniciarTimer();
        this.cargando.set(false);
        this.snackBar.open('Entrevista iniciada', 'OK', { duration: 2000 });
      },
      error: (err) => {
        this.cargando.set(false);
        const msg = err?.error?.message || 'Error al iniciar';
        this.snackBar.open(typeof msg === 'string' ? msg : 'Error', 'OK', { duration: 3000 });
      }
    });
  }

  togglePausa(): void {
    if (this.grabando()) {
      this.audioService.detenerGrabacion().then(result => {
        if (result.texto) this.transcripcionTexto.set(result.texto);
        if (result.audioBlob) this.ultimoAudioBlob.set(result.audioBlob);
      });
    }
    this.sesionPausada.update(v => !v);
  }

  cancelarSesion(): void {
    if (!confirm('¿Cancelar la sesión? La sesión será eliminada y no podrá recuperarse. Las respuestas registradas hasta ahora se perderán.')) return;

    this.cancelando.set(true);
    this.sesionService.cancelarPorEvaluador(this.sesionId).subscribe({
      next: () => {
        this.cancelando.set(false);
        clearInterval(this.timerInterval);
        this.snackBar.open('Sesión cancelada', 'OK', { duration: 3000 });
        this.volver();
      },
      error: (err) => {
        this.cancelando.set(false);
        const msg = err?.error?.message || 'Error al cancelar';
        this.snackBar.open(typeof msg === 'string' ? msg : 'Error', 'OK', { duration: 3000 });
      }
    });
  }

  async toggleGrabacion(): Promise<void> {
    if (this.grabando()) {
      const result = await this.audioService.detenerGrabacion();
      this.transcripcionTexto.set(result.texto);
      this.ultimoAudioBlob.set(result.audioBlob);

      // If Web Speech API got nothing but we have audio, try Whisper
      if (!result.texto && result.audioBlob) {
        this.enviarAWhisper(result.audioBlob);
      }
    } else {
      this.transcripcionTexto.set('');
      this.ultimoAudioBlob.set(null);
      this.audioService.resetear();
      await this.audioService.iniciarGrabacion('es-ES');
    }
  }

  enviarAWhisper(blob?: Blob): void {
    const audioBlob = blob ?? this.ultimoAudioBlob();
    if (!audioBlob) return;

    this.transcribiendoWhisper.set(true);
    this.sesionService.transcribir(this.sesionId, audioBlob).subscribe({
      next: res => {
        if (res.transcripcion) {
          this.transcripcionTexto.set(res.transcripcion);
        }
        this.transcribiendoWhisper.set(false);
      },
      error: () => {
        this.snackBar.open('Error en transcripción Whisper', 'OK', { duration: 3000 });
        this.transcribiendoWhisper.set(false);
      }
    });
  }

  enviarRespuesta(): void {
    const p = this.preguntaActual();
    const texto = this.transcripcionTexto();
    if (!p || !texto.trim()) return;

    this.enviandoRespuesta.set(true);
    const tiempoUsado = this.segundosTimer();

    this.sesionService.responderPorEvaluador(this.sesionId, {
      preguntaId: p.preguntaId,
      contenido: texto.trim(),
      tiempoUsadoSegundos: tiempoUsado
    }).subscribe({
      next: (res: any) => {
        this.marcarRespondida(p.index, texto.trim());
        this.enviandoRespuesta.set(false);
        this.transcripcionTexto.set('');
        this.ultimoAudioBlob.set(null);
        this.audioService.resetear();

        if (res.completada) {
          this.completarSesion();
        } else {
          this.preguntaActual.set(res as PreguntaEnVivoDto);
          this.agregarHistorial(res as PreguntaEnVivoDto);
          this.snackBar.open(`Pregunta ${(res as PreguntaEnVivoDto).index + 1} de ${(res as PreguntaEnVivoDto).totalPreguntas}`, 'OK', { duration: 2000 });
        }
      },
      error: (err) => {
        this.enviandoRespuesta.set(false);
        const msg = err?.error?.message || 'Error al enviar respuesta';
        this.snackBar.open(typeof msg === 'string' ? msg : 'Error', 'OK', { duration: 3000 });
      }
    });
  }

  finalizarEntrevista(): void {
    if (!confirm('¿Finalizar la entrevista? Las preguntas sin responder no serán evaluadas.')) return;

    this.sesionService.finalizarPorEvaluador(this.sesionId).subscribe({
      next: () => {
        this.completarSesion();
      },
      error: (err) => {
        const msg = err?.error?.message || 'Error al finalizar';
        this.snackBar.open(typeof msg === 'string' ? msg : 'Error', 'OK', { duration: 3000 });
      }
    });
  }

  irAResultado(): void {
    const candidatoId = this.resultadoCandidatoId() ?? this.estado()?.candidatoId;
    if (candidatoId && this.evaluacionId) {
      this.router.navigate(['/evaluador/evaluacion', this.evaluacionId, 'resultado', candidatoId]);
    }
  }

  private completarSesion(): void {
    this.sesionCompletada.set(true);
    this.sesionIniciada.set(false);
    clearInterval(this.timerInterval);
    this.snackBar.open('Entrevista completada — iniciando análisis IA...', '', { duration: 3000 });
    this.ejecutarAnalisis();
  }

  private ejecutarAnalisis(): void {
    const candidatoId = this.estado()?.candidatoId;
    if (!candidatoId) return;

    this.analizando.set(true);
    this.resultadoService.ejecutarAnalisis(candidatoId).subscribe({
      next: () => {
        this.analizando.set(false);
        this.analisisCompletado.set(true);
        this.resultadoCandidatoId.set(candidatoId);
      },
      error: () => {
        this.analizando.set(false);
        // Analysis failed but session is done — don't block the user
        this.snackBar.open('El análisis IA no pudo completarse. Puede ejecutarlo manualmente desde la evaluación.', 'OK', { duration: 6000 });
      }
    });
  }

  private cargarPreguntaActual(): void {
    this.sesionService.obtenerPreguntaActualEvaluador(this.sesionId).subscribe({
      next: pregunta => {
        this.preguntaActual.set(pregunta);
        this.agregarHistorial(pregunta);
      }
    });
  }

  private iniciarTimer(): void {
    clearInterval(this.timerInterval);
    this.timerInterval = setInterval(() => {
      if (!this.sesionPausada()) {
        this.segundosTimer.update(v => v + 1);
      }
    }, 1000);
  }

  private agregarHistorial(p: PreguntaEnVivoDto): void {
    const current = this.historial();
    if (!current.find(h => h.index === p.index)) {
      this.historial.set([...current, { index: p.index, texto: p.texto, respondida: false }]);
    }
  }

  private marcarRespondida(index: number, transcripcion: string): void {
    this.historial.update(list =>
      list.map(h => h.index === index ? { ...h, respondida: true, transcripcion } : h)
    );
  }

  formatTiempo(seg: number): string {
    const m = Math.floor(seg / 60);
    const s = seg % 60;
    return `${m.toString().padStart(2, '0')}:${s.toString().padStart(2, '0')}`;
  }

  volver(): void {
    this.router.navigate(['/evaluador/evaluacion', this.evaluacionId]);
  }
}
