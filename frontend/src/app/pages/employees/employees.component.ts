import { Component, inject, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { MatTableModule, MatTableDataSource } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { environment } from '../../../environments/environment';

@Component({
  selector: 'app-employees',
  standalone: true,
  imports: [MatTableModule, MatButtonModule, MatIconModule],
  template: `
    <h1>Employés</h1>
    <button mat-flat-button color="primary">Nouvel employé</button>
    <table mat-table [dataSource]="dataSource" class="mat-elevation-z2">
      <ng-container matColumnDef="name">
        <th mat-header-cell *matHeaderCellDef>Nom</th>
        <td mat-cell *matCellDef="let e">{{ e.firstName }} {{ e.lastName }}</td>
      </ng-container>
      <ng-container matColumnDef="email">
        <th mat-header-cell *matHeaderCellDef>Email</th>
        <td mat-cell *matCellDef="let e">{{ e.email }}</td>
      </ng-container>
      <ng-container matColumnDef="contract">
        <th mat-header-cell *matHeaderCellDef>Contrat</th>
        <td mat-cell *matCellDef="let e">{{ e.contractType }}</td>
      </ng-container>
      <ng-container matColumnDef="actions">
        <th mat-header-cell *matHeaderCellDef></th>
        <td mat-cell *matCellDef="let e">
          <button mat-icon-button><mat-icon>edit</mat-icon></button>
        </td>
      </ng-container>
      <tr mat-header-row *matHeaderRowDef="['name','email','contract','actions']"></tr>
      <tr mat-row *matRowDef="let row; columns: ['name','email','contract','actions']"></tr>
    </table>
  `,
  styles: ['table { width: 100%; }', 'button { margin-bottom: 1rem; }']
})
export class EmployeesComponent {
  private http = inject(HttpClient);
  dataSource = new MatTableDataSource<any>([]);
  displayedColumns = ['name', 'email', 'contract', 'actions'];

  ngOnInit() {
    this.http.get<any[]>(`${environment.apiUrl}/employees`).subscribe((d) => (this.dataSource.data = d));
  }
}
