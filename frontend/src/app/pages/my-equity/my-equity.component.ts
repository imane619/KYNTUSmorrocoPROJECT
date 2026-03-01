import { Component } from '@angular/core';
import { MatCardModule } from '@angular/material/card';
import { MatProgressBarModule } from '@angular/material/progress-bar';

@Component({
  selector: 'app-my-equity',
  standalone: true,
  imports: [MatCardModule, MatProgressBarModule],
  template: `
    <h1>Mon Score Équité</h1>
    <mat-card>
      <mat-card-title>Score actuel</mat-card-title>
      <mat-card-content>
        <p class="score">85%</p>
        <mat-progress-bar mode="determinate" value="85"></mat-progress-bar>
      </mat-card-content>
    </mat-card>
  `,
  styles: ['.score { font-size: 2rem; font-weight: bold; color: #2196f3; }']
})
export class MyEquityComponent {}
