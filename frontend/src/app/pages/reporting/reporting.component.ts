import { Component } from '@angular/core';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';

@Component({
  selector: 'app-reporting',
  standalone: true,
  imports: [MatCardModule, MatButtonModule],
  template: `
    <h1>Reporting</h1>
    <mat-card>
      <mat-card-title>Rapports</mat-card-title>
      <mat-card-content>
        <button mat-flat-button>Export PDF</button>
        <button mat-flat-button>Export Excel</button>
      </mat-card-content>
    </mat-card>
  `
})
export class ReportingComponent {}
