import { Component, input, computed } from '@angular/core'; // Ajout de computed
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-equity-widget',
  standalone: true,
  imports: [RouterLink],
  template: `
    <div class="equity-widget glass-card">
      <div class="donut-container">
        <svg viewBox="0 0 120 120" class="donut">
          <circle cx="60" cy="60" r="54" fill="none" stroke="rgba(255,255,255,0.1)" stroke-width="12"/>
          
          <circle cx="60" cy="60" r="54" fill="none" stroke="#f59e0b" stroke-width="12"
            [attr.stroke-dasharray]="circumference" 
            [attr.stroke-dashoffset]="offset()"
            stroke-linecap="round" 
            transform="rotate(-90 60 60)"/>
        </svg>
        
        <div class="donut-center">
          <span class="score">{{ score() }}%</span>
          <span class="label">{{ label() }}</span>
        </div>
      </div>

      @if (trend()) {
        <p class="trend" [class.positive]="trend()?.startsWith('+')">{{ trend() }} ce mois</p>
      }
      
      <a routerLink="/my-equity" class="link">Voir détails</a>
    </div>
  `,
  styles: [`
    .equity-widget {
      padding: 1.5rem;
      border-radius: 16px;
      background: rgba(255, 255, 255, 0.08);
      backdrop-filter: blur(12px);
      border: 1px solid rgba(255, 255, 255, 0.12);
      box-shadow: 0 8px 32px rgba(0, 0, 0, 0.2);
      color: white; /* Assure la visibilité sur fond sombre */
    }
    .donut-container {
      position: relative;
      width: 120px;
      height: 120px;
      margin: 0 auto 1rem;
    }
    .donut {
      width: 100%;
      height: 100%;
    }
    .donut-center {
      position: absolute;
      inset: 0;
      display: flex;
      flex-direction: column;
      align-items: center;
      justify-content: center;
    }
    .score {
      font-size: 1.75rem;
      font-weight: 700;
      color: #f59e0b;
    }
    .label {
      font-size: 0.75rem;
      opacity: 0.9;
    }
    .trend {
      font-size: 0.875rem;
      margin: 0;
      text-align: center;
    }
    .trend.positive { color: #10b981; }
    .link {
      display: block;
      text-align: center;
      margin-top: 0.75rem;
      font-size: 0.875rem;
      color: #60a5fa;
      text-decoration: none;
    }
    .link:hover { text-decoration: underline; }
  `]
})
export class EquityWidgetComponent {
  score = input.required<number>();
  label = input<string>('Très bien');
  trend = input<string>();

  readonly circumference = 2 * Math.PI * 54;

  // Utilisation de computed pour une performance optimale avec les Signals
  offset = computed(() => {
    const pct = this.score() / 100;
    return this.circumference * (1 - pct);
  });
}