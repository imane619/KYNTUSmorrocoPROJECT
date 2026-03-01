import { Component } from '@angular/core';
import { MatCardModule } from '@angular/material/card';

@Component({
  selector: 'app-settings',
  standalone: true,
  imports: [MatCardModule],
  template: `
    <h1>Paramètres</h1>
    <mat-card>
      <mat-card-title>Configuration</mat-card-title>
      <mat-card-content>
        <p>Paramètres de l'application.</p>
      </mat-card-content>
    </mat-card>
  `
})
export class SettingsComponent {}
