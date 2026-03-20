import { Component, computed, effect, input, output, signal, OnDestroy } from '@angular/core';
import { MatIconModule } from '@angular/material/icon';

@Component({
  selector: 'app-countdown-timer',
  imports: [MatIconModule],
  templateUrl: './countdown-timer.component.html',
  styleUrl: './countdown-timer.component.scss',
})
export class CountdownTimerComponent implements OnDestroy {
  segundosRestantes = input.required<number>();
  autoIniciar = input<boolean>(true);
  tiempoAgotado = output<void>();

  segundosActuales = signal(0);
  private intervalId: ReturnType<typeof setInterval> | null = null;

  display = computed(() => {
    const total = this.segundosActuales();
    const min = Math.floor(total / 60);
    const seg = total % 60;
    return `${min.toString().padStart(2, '0')}:${seg.toString().padStart(2, '0')}`;
  });

  urgente = computed(() => this.segundosActuales() <= 60 && this.segundosActuales() > 0);
  agotado = computed(() => this.segundosActuales() <= 0);

  constructor() {
    effect(() => {
      const segs = this.segundosRestantes();
      this.segundosActuales.set(segs);
      this.detener();
      if (this.autoIniciar() && segs > 0) this.iniciar();
    });
  }

  iniciar(): void {
    this.detener();
    this.intervalId = setInterval(() => {
      const actual = this.segundosActuales();
      if (actual <= 1) {
        this.segundosActuales.set(0);
        this.detener();
        this.tiempoAgotado.emit();
        return;
      }
      this.segundosActuales.set(actual - 1);
    }, 1000);
  }

  detener(): void {
    if (this.intervalId) {
      clearInterval(this.intervalId);
      this.intervalId = null;
    }
  }

  ngOnDestroy(): void {
    this.detener();
  }
}
