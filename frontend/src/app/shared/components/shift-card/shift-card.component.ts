import { Component, input } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { RouterLink } from '@angular/router';

export interface ShiftCardData {
  shiftCode: string;
  startTime: string;
  endTime: string;
  cellule?: string;
  status: 'En cours' | 'Terminé' | 'À venir';
}

@Component({
  selector: 'app-shift-card',
  standalone: true,
  imports: [MatButtonModule, RouterLink],
  templateUrl: './shift-card.component.html',
  styleUrl: './shift-card.component.scss'
})
export class ShiftCardComponent {
  data = input.required<ShiftCardData>();

  shiftColor(): string {
    const code = this.data()?.shiftCode?.toUpperCase() ?? '';
    const colors: Record<string, string> = {
      A: '#10b981',
      B: '#3b82f6',
      C: '#8b5cf6',
      D: '#f59e0b',
    };
    return colors[code] ?? '#64748b';
  }
}
