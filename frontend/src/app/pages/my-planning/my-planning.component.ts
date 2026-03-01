import { Component } from '@angular/core';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';

@Component({
  selector: 'app-my-planning',
  standalone: true,
  imports: [MatCardModule, MatButtonModule],
  template: `
    <h1>Mon Planning</h1>
    <mat-card>
      <mat-card-title>Planning de la semaine</mat-card-title>
      <mat-card-content>
        <p>9h - 16h (Lun-Ven)</p>
        <button mat-flat-button color="primary">Voir planning</button>
      </mat-card-content>
    </mat-card>
  `
})
export class MyPlanningComponent {}
