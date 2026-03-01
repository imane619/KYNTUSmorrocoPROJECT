import { Component } from '@angular/core';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';

@Component({
  selector: 'app-leaves',
  standalone: true,
  imports: [MatCardModule, MatButtonModule],
  template: `
    <h1>Congés</h1>
    <mat-card>
      <mat-card-title>Demandes en attente</mat-card-title>
      <mat-card-content>
        <p>Workflow d'approbation des congés.</p>
      </mat-card-content>
    </mat-card>
  `
})
export class LeavesComponent {}
