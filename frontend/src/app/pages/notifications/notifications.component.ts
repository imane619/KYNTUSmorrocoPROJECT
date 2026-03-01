import { Component } from '@angular/core';
import { MatCardModule } from '@angular/material/card';
import { MatListModule } from '@angular/material/list';

@Component({
  selector: 'app-notifications',
  standalone: true,
  imports: [MatCardModule, MatListModule],
  template: `
    <h1>Notifications</h1>
    <mat-card>
      <mat-list>
        <mat-list-item>Planning validé</mat-list-item>
        <mat-list-item>Demande de congé approuvée</mat-list-item>
      </mat-list>
    </mat-card>
  `
})
export class NotificationsComponent {}
