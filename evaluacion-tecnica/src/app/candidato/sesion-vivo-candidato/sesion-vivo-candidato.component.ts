import { Component, OnInit, OnDestroy, signal, computed, inject } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatSnackBar } from '@angular/material/snack-bar';
import { SesionService } from '../../core/services/sesion/sesion.service';
import { SignalRService } from '../../core/services/signalr/signalr.service';
import { AudioTranscripcionService } from '../../core/services/audio/audio-transcripcion.service';
import { PreguntaEnVivoDto } from '../../core/models/sesion.model';

@Component({
  selector: 'app-sesion-vivo-candidato',
  imports: [
    MatCardModule, MatButtonModule, MatIconModule,
    MatProgressSpinnerModule, MatProgressBarModule
  ],
  templateUrl: './sesion-vivo-candidato.component.html',
  styleUrl: './sesion-vivo-candidato.component.scss',
})
export class SesionVivoCandidatoComponent implements OnInit, OnDestroy {
  pregunta = signal<PreguntaEnVivoDto | null>(null);
  cargando = signal(true);
  iniciando = signal(false);
  respondiendo = signal(false);
  sesionCompletada = signal(false);
  permisoMicrofono = signal(false);
  segundosPregunta = signal(0);
  preguntasRespondidas = signal(0);

  private audioService = inject(AudioTranscripcionService);
  private sesionService = inject(SesionService);
  private signalR = inject(SignalRService);
  private snackBar = inject(MatSnackBar);
  private route = inject(ActivatedRoute);
  private router = inject(Router);

  grabando = this.audioService.grabando;
  transcripcionParcial = this.audioService.transcripcionParcial;
  transcripcionFinal = this.audioService.transcripcionFinal;
  nivelAudio = this.audioService.nivelAudio;
  soportado = this.audioService.soportado;

  progreso = computed(() => {
    const p = this.pregunta();
    if (!p) return 0;
    return Math.round(((this.preguntasRespondidas()) / p.totalPreguntas) * 100);
  });

  tiempoRestante = computed(() => {
    const p = this.pregunta();
    if (!p || p.tiempoLimiteSegundos <= 0) return -1;
    return Math.max(0, p.tiempoLimiteSegundos - this.segundosPregunta());
  });

  private sesionId = '';
  private token = '';
  private timerInterval: any;

  constructor() {}

  ngOnInit(): void {
    this.sesionId = this.route.snapshot.paramMap.get('sId') ?? '';
    // Token from parent route (candidato/:token/sesion-vivo/:sId)
    this.token = this.route.parent?.snapshot.paramMap.get('token') ?? '';

    this.verificarPermisoMicrofono();
    this.cargando.set(false);
  }

  ngOnDestroy(): void {
    clearInterval(this.timerInterval);
    this.signalR.desconectar();
    if (this.grabando()) {
      this.audioService.detenerGrabacion();
    }
  }

  private async verificarPermisoMicrofono(): Promise<void> {
    try {
      const stream = await navigator.mediaDevices.getUserMedia({ audio: true });
      stream.getTracks().forEach(t => t.stop());
      this.permisoMicrofono.set(true);
    } catch {
      this.permisoMicrofono.set(false);
    }
  }

  async iniciarSesion(): Promise<void> {
    if (!this.permisoMicrofono()) {
      await this.verificarPermisoMicrofono();
      if (!this.permisoMicrofono()) {
        this.snackBar.open('Se requiere acceso al micrófono para la entrevista', 'OK', { duration: 4000 });
        return;
      }
    }

    this.iniciando.set(true);

    // Connect SignalR
    try {
      await this.signalR.conectar(this.sesionId);
    } catch {
      this.snackBar.open('Error de conexión en tiempo real', 'OK', { duration: 3000 });
    }

    // Start session via API
    this.sesionService.iniciar(this.sesionId, this.token).subscribe({
      next: async (pregunta) => {
        this.pregunta.set(pregunta);
        this.iniciando.set(false);
        this.segundosPregunta.set(0);
        this.iniciarTimer();
        // Auto-start recording for the first question
        await this.iniciarGrabacion();
      },
      error: (err) => {
        const msg = err?.error?.message || 'Error al iniciar sesión';
        this.snackBar.open(typeof msg === 'string' ? msg : 'Error', 'OK', { duration: 4000 });
        this.iniciando.set(false);
      }
    });
  }

  async iniciarGrabacion(): Promise<void> {
    try {
      this.audioService.resetear();
      await this.audioService.iniciarGrabacion('es-ES');
    } catch {
      this.snackBar.open('No se pudo acceder al micrófono', 'OK', { duration: 3000 });
    }
  }

  async enviarRespuesta(): Promise<void> {
    const p = this.pregunta();
    if (!p || this.respondiendo()) return;

    this.respondiendo.set(true);

    // Stop recording and get transcription
    const { texto } = await this.audioService.detenerGrabacion();
    const contenido = texto || '(Sin respuesta audible)';
    const tiempoUsado = this.segundosPregunta();

    clearInterval(this.timerInterval);

    this.sesionService.responder(this.sesionId, {
      preguntaId: p.preguntaId,
      contenido,
      tiempoUsadoSegundos: tiempoUsado
    }, this.token).subscribe({
      next: async (siguiente) => {
        this.preguntasRespondidas.update(v => v + 1);
        this.respondiendo.set(false);

        if (!siguiente || !(siguiente as PreguntaEnVivoDto).preguntaId) {
          // Session completed
          this.sesionCompletada.set(true);
          this.pregunta.set(null);
          this.snackBar.open('Entrevista completada', 'OK', { duration: 3000 });
          return;
        }

        // Next question
        this.pregunta.set(siguiente as PreguntaEnVivoDto);
        this.segundosPregunta.set(0);
        this.iniciarTimer();
        await this.iniciarGrabacion();
      },
      error: () => {
        this.snackBar.open('Error al enviar respuesta', 'OK', { duration: 3000 });
        this.respondiendo.set(false);
      }
    });
  }

  async finalizarSesion(): Promise<void> {
    if (this.grabando()) {
      await this.audioService.detenerGrabacion();
    }
    clearInterval(this.timerInterval);

    this.sesionService.finalizar(this.sesionId, this.token).subscribe({
      next: () => {
        this.sesionCompletada.set(true);
        this.pregunta.set(null);
      },
      error: () => this.snackBar.open('Error al finalizar', 'OK', { duration: 3000 })
    });
  }

  private iniciarTimer(): void {
    clearInterval(this.timerInterval);
    this.timerInterval = setInterval(() => {
      this.segundosPregunta.update(v => v + 1);

      // Auto-submit if time limit exceeded
      const p = this.pregunta();
      if (p && p.tiempoLimiteSegundos > 0 && this.segundosPregunta() >= p.tiempoLimiteSegundos) {
        this.enviarRespuesta();
      }
    }, 1000);
  }

  formatTiempo(seg: number): string {
    const m = Math.floor(seg / 60);
    const s = seg % 60;
    return `${m.toString().padStart(2, '0')}:${s.toString().padStart(2, '0')}`;
  }
}
