import { Component } from '@angular/core';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';

@Component({
  selector: 'app-planning',
  standalone: true,
  imports: [MatCardModule, MatButtonModule],
  template: `
    <h1>Planning</h1>
    <mat-card>
      <mat-card-title>Génération de planning</mat-card-title>
      <mat-card-content>
        <p>Mode simulation (Sandbox) disponible. Générez un planning pour comparer les scénarios.</p>
        <button mat-flat-button color="primary">Générer planning</button>
      </mat-card-content>
    </mat-card>
  `
})
export class PlanningComponent {}
