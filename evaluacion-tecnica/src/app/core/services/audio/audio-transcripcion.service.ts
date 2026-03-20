import { Injectable, signal } from '@angular/core';

export interface TranscripcionResult {
  texto: string;
  confianza: number;
  esFinal: boolean;
}

@Injectable({ providedIn: 'root' })
export class AudioTranscripcionService {
  readonly grabando = signal(false);
  readonly transcripcionParcial = signal('');
  readonly transcripcionFinal = signal('');
  readonly nivelAudio = signal(0);
  readonly soportado = signal(this.verificarSoporte());

  private recognition: any = null;
  private mediaRecorder: MediaRecorder | null = null;
  private audioChunks: Blob[] = [];
  private audioContext: AudioContext | null = null;
  private analyser: AnalyserNode | null = null;
  private animFrameId = 0;
  private stream: MediaStream | null = null;

  private verificarSoporte(): boolean {
    return !!(window as any).webkitSpeechRecognition || !!(window as any).SpeechRecognition;
  }

  async iniciarGrabacion(idioma = 'es-ES'): Promise<void> {
    if (this.grabando()) return;

    // Request microphone
    this.stream = await navigator.mediaDevices.getUserMedia({ audio: true });

    // Setup audio level monitoring
    this.audioContext = new AudioContext();
    const source = this.audioContext.createMediaStreamSource(this.stream);
    this.analyser = this.audioContext.createAnalyser();
    this.analyser.fftSize = 256;
    source.connect(this.analyser);
    this.monitorearNivel();

    // Setup MediaRecorder for backup recording
    this.audioChunks = [];
    this.mediaRecorder = new MediaRecorder(this.stream, { mimeType: this.obtenerMimeType() });
    this.mediaRecorder.ondataavailable = (e) => {
      if (e.data.size > 0) this.audioChunks.push(e.data);
    };
    this.mediaRecorder.start(1000);

    // Setup Speech Recognition
    const SpeechRecognition = (window as any).webkitSpeechRecognition || (window as any).SpeechRecognition;
    this.recognition = new SpeechRecognition();
    this.recognition.continuous = true;
    this.recognition.interimResults = true;
    this.recognition.lang = idioma;
    this.recognition.maxAlternatives = 1;

    let textoAcumulado = '';

    this.recognition.onresult = (event: any) => {
      let interim = '';
      let finalText = '';

      for (let i = event.resultIndex; i < event.results.length; i++) {
        const transcript = event.results[i][0].transcript;
        if (event.results[i].isFinal) {
          finalText += transcript + ' ';
        } else {
          interim += transcript;
        }
      }

      if (finalText) {
        textoAcumulado += finalText;
        this.transcripcionFinal.set(textoAcumulado.trim());
      }
      this.transcripcionParcial.set(interim);
    };

    this.recognition.onerror = (event: any) => {
      if (event.error === 'no-speech') return; // Ignore silence
      console.error('Speech recognition error:', event.error);
    };

    this.recognition.onend = () => {
      // Auto-restart if still recording
      if (this.grabando() && this.recognition) {
        try { this.recognition.start(); } catch {}
      }
    };

    this.recognition.start();
    this.grabando.set(true);
    this.transcripcionFinal.set('');
    this.transcripcionParcial.set('');
  }

  async detenerGrabacion(): Promise<{ texto: string; audioBlob: Blob | null }> {
    this.grabando.set(false);
    const texto = this.transcripcionFinal();

    // Stop recognition
    if (this.recognition) {
      try { this.recognition.stop(); } catch {}
      this.recognition = null;
    }

    // Stop media recorder
    let audioBlob: Blob | null = null;
    if (this.mediaRecorder && this.mediaRecorder.state !== 'inactive') {
      await new Promise<void>(resolve => {
        this.mediaRecorder!.onstop = () => resolve();
        this.mediaRecorder!.stop();
      });
      audioBlob = new Blob(this.audioChunks, { type: this.obtenerMimeType() });
    }

    // Cleanup audio monitoring
    cancelAnimationFrame(this.animFrameId);
    this.audioContext?.close();
    this.audioContext = null;
    this.analyser = null;
    this.nivelAudio.set(0);

    // Stop microphone stream
    this.stream?.getTracks().forEach(t => t.stop());
    this.stream = null;

    return { texto, audioBlob };
  }

  resetear(): void {
    this.transcripcionFinal.set('');
    this.transcripcionParcial.set('');
  }

  private monitorearNivel(): void {
    if (!this.analyser) return;
    const data = new Uint8Array(this.analyser.frequencyBinCount);

    const tick = () => {
      if (!this.analyser) return;
      this.analyser.getByteFrequencyData(data);
      const avg = data.reduce((a, b) => a + b, 0) / data.length;
      this.nivelAudio.set(Math.min(100, Math.round((avg / 128) * 100)));
      this.animFrameId = requestAnimationFrame(tick);
    };
    tick();
  }

  private obtenerMimeType(): string {
    if (MediaRecorder.isTypeSupported('audio/webm;codecs=opus')) return 'audio/webm;codecs=opus';
    if (MediaRecorder.isTypeSupported('audio/webm')) return 'audio/webm';
    return 'audio/ogg';
  }
}
