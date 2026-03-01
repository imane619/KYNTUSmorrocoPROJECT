import { Component, inject, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { AuthService } from '../../core/services/auth.service';
import { environment } from '../../../environments/environment';

interface DashboardKpi {
  coverageRate: number;
  activeEmployees: number;
  onBreak: number;
  absent: number;
  equityScore: number;
}

interface HeatmapData {
  rowLabels: string[];
  columnLabels: string[];
  values: number[][];
}

interface Alert {
  id: string;
  type: string;
  message: string;
  severity: string;
}

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [MatCardModule, MatIconModule, MatProgressBarModule, MatButtonModule],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.scss'
})
export class DashboardComponent {
  private http = inject(HttpClient);
  private router = inject(Router);
  auth = inject(AuthService);

  isEmployee = this.auth.hasRole('Employee');
  kpis = signal<DashboardKpi | null>(null);
  heatmap = signal<HeatmapData | null>(null);
  alerts = signal<Alert[]>([]);

  constructor() {
    if (this.isEmployee) {
      this.loadEmployeeDashboard();
    } else {
      this.loadManagerDashboard();
    }
  }

  private loadManagerDashboard(): void {
    this.http.get<DashboardKpi>(`${environment.apiUrl}/api/analytics/dashboard/kpis`).subscribe((k) => this.kpis.set(k));
    this.http.get<HeatmapData>(`${environment.apiUrl}/api/analytics/dashboard/heatmap`).subscribe((h) => this.heatmap.set(h));
    this.http.get<Alert[]>(`${environment.apiUrl}/api/analytics/dashboard/alerts`).subscribe((a) => this.alerts.set(a));
  }

  private loadEmployeeDashboard(): void {
    this.kpis.set({
      coverageRate: 0,
      activeEmployees: 0,
      onBreak: 0,
      absent: 0,
      equityScore: 85
    });
  }

  getHeatmapColor(val: number): string {
    if (val === 0) return '#f44336';
    if (val === 1) return '#ff9800';
    return '#4caf50';
  }
}
