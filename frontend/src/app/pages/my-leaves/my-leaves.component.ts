import { Component } from '@angular/core';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';

@Component({
  selector: 'app-my-leaves',
  standalone: true,
  imports: [MatCardModule, MatButtonModule],
  template: `
    <h1>Mes Congés</h1>
    <mat-card>
      <mat-card-title>Demander un congé</mat-card-title>
      <mat-card-content>
        <p>Envoyez une demande d'absence</p>
        <button mat-flat-button color="accent">Nouvelle demande</button>
      </mat-card-content>
    </mat-card>
  `
})
export class MyLeavesComponent {}
