import { Component, input } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-leave-summary',
  standalone: true,
  imports: [MatButtonModule, MatIconModule, RouterLink],
  template: `
    <div class="leave-summary glass-card">
      <h3 class="title">Solde Congé</h3>
      <p class="days">{{ remaining() }} jours restants</p>
      <div class="progress-bar">
        <div class="progress-fill" [style.width.%]="progress()"></div>
      </div>
      @if (nextLeave()) {
        <p class="next">Prochain: {{ nextLeave() }}</p>
      }
      <a mat-flat-button color="accent" routerLink="/my-leaves/new" class="mt-3">Demander congé</a>
    </div>
  `,
  styles: [`
    .leave-summary {
      padding: 1.5rem;
      border-radius: 16px;
      background: rgba(255, 255, 255, 0.08);
      backdrop-filter: blur(12px);
      border: 1px solid rgba(255, 255, 255, 0.12);
      box-shadow: 0 8px 32px rgba(0, 0, 0, 0.2);
    }
    .title { font-size: 1rem; margin: 0 0 0.5rem; opacity: 0.9; }
    .days { font-size: 1.5rem; font-weight: 700; margin: 0; }
    .progress-bar {
      height: 8px;
      background: rgba(255,255,255,0.15);
      border-radius: 9999px;
      overflow: hidden;
      margin: 1rem 0;
    }
    .progress-fill {
      height: 100%;
      background: linear-gradient(90deg, #3b82f6, #60a5fa);
      border-radius: 9999px;
      transition: width 0.3s ease;
    }
    .next { font-size: 0.875rem; opacity: 0.8; margin: 0; }
    .mt-3 { margin-top: 1rem; }
  `]
})
export class LeaveSummaryComponent {
  remaining = input.required<number>();
  total = input<number>(15);
  nextLeave = input<string>();

  progress(): number {
    const t = this.total();
    const r = this.remaining();
    return t > 0 ? (r / t) * 100 : 0;
  }
}
