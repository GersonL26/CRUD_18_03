import { Component, computed, input } from '@angular/core';
import { MatIconModule } from '@angular/material/icon';

@Component({
  selector: 'app-score-badge',
  imports: [MatIconModule],
  templateUrl: './score-badge.component.html',
  styleUrl: './score-badge.component.scss',
})
export class ScoreBadgeComponent {
  score = input.required<number>();
  size = input<'sm' | 'md' | 'lg'>('md');

  categoria = computed(() => {
    const s = this.score();
    if (s >= 80) return 'alto';
    if (s >= 50) return 'medio';
    return 'bajo';
  });

  icono = computed(() => {
    const cat = this.categoria();
    if (cat === 'alto') return 'check_circle';
    if (cat === 'medio') return 'warning';
    return 'cancel';
  });
}
