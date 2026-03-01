import { Component } from '@angular/core';
import { MatCardModule } from '@angular/material/card';

@Component({
  selector: 'app-analytics',
  standalone: true,
  imports: [MatCardModule],
  template: `
    <h1>Analytics</h1>
    <mat-card>
      <mat-card-title>Performance Équipe</mat-card-title>
      <mat-card-content>
        <p>Taux de rotation, pauses critiques, évolution de la couverture.</p>
      </mat-card-content>
    </mat-card>
  `
})
export class AnalyticsComponent {}
