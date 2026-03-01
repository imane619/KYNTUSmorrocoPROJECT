import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatTableModule } from '@angular/material/table';
import { MatSnackBar } from '@angular/material/snack-bar';

@Component({
  selector: 'app-leaves',
  standalone: true,
  imports: [CommonModule, MatCardModule, MatButtonModule, MatTableModule],
  template: `
    <div class="p-6">
      <h1 class="text-2xl font-bold mb-4">Gestion des Congés</h1>
      
      <mat-card class="p-4">
        <mat-card-title class="mb-4">Demandes en attente d'approbation</mat-card-title>
        
        <table mat-table [dataSource]="pendingLeaves()" class="w-full">
          
          <ng-container matColumnDef="employee">
            <th mat-header-cell *matHeaderCellDef> Employé </th>
            <td mat-cell *matCellDef="let element"> {{ element.name }} </td>
          </ng-container>

          <ng-container matColumnDef="dates">
            <th mat-header-cell *matHeaderCellDef> Période </th>
            <td mat-cell *matCellDef="let element"> 
              {{ element.start }} - {{ element.end }} 
              <strong>({{ element.days }}j)</strong>
            </td>
          </ng-container>

          <ng-container matColumnDef="actions">
            <th mat-header-cell *matHeaderCellDef> Décision </th>
            <td mat-cell *matCellDef="let element">
              <button mat-flat-button color="primary" class="mr-2" (click)="decide(element.id, 'Approuvée')">
                Approuver
              </button>
              <button mat-stroked-button color="warn" (click)="decide(element.id, 'Rejetée')">
                Rejeter
              </button>
            </td>
          </ng-container>

          <tr mat-header-row *matHeaderRowDef="displayedColumns"></tr>
          <tr mat-row *matRowDef="let row; columns: displayedColumns;"></tr>
        </table>

        @if (pendingLeaves().length === 0) {
          <p class="py-4 text-center text-gray-500">Aucune demande en attente.</p>
        }
      </mat-card>
    </div>
  `
})
export class LeavesComponent {
  private snackBar = inject(MatSnackBar);
  
  displayedColumns: string[] = ['employee', 'dates', 'actions'];

  // Simulation des données qui viendront de Absence.API
  pendingLeaves = signal([
    { id: 101, name: 'Jean Employé', type: 'Payé', start: '01/03/2026', end: '03/03/2026', days: 2 },
    { id: 102, name: 'Alice Dev', type: 'Absence', start: '10/03/2026', end: '12/03/2026', days: 3 },
    { 
    id: 202, 
    name: 'Ton Nom Employé', 
    type: 'Payé', 
    start: '02/03/2026', 
    end: '05/03/2026', 
    days: 4 
  }
  ]);

  decide(id: number, status: string) {
    // 1. Logique : Retirer de la liste visuelle
    this.pendingLeaves.update(list => list.filter(item => item.id !== id));
    
    // 2. Feedback visuel
    this.snackBar.open(`Demande ${status} avec succès !`, 'Fermer', { duration: 3000 });

    // 3. Prochaine étape : Appel API vers ton microservice pour mettre à jour le statut
    console.log(`Appel API Absence: Demande ${id} passée à ${status}`);
  }
}