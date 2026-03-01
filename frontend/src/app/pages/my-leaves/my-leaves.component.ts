import { Component, inject } from '@angular/core'; 
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { Router } from '@angular/router'; 

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
        <button mat-flat-button color="primary" (click)="onNewAbsenceRequest()">
          Nouvelle demande
        </button>
      </mat-card-content>
    </mat-card>
  `
})
export class MyLeavesComponent {
  private router = inject(Router);

  onNewAbsenceRequest() {
    console.log("Redirection vers le formulaire...");
    this.router.navigate(['/my-leaves/new']);
  }
}