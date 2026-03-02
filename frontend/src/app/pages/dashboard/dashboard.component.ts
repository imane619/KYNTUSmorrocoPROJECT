import { Component, inject, signal, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { AuthService } from '../../core/services/auth.service';
import { DashboardService } from '../../core/services/dashboard.service';
import { ShiftCardComponent } from '../../shared/components/shift-card/shift-card.component';
import { EquityWidgetComponent } from '../../shared/components/equity-widget/equity-widget.component';
import { LeaveSummaryComponent } from '../../shared/components/leave-summary/leave-summary.component';
import { environment } from '../../../environments/environment';
import type { ShiftCardData } from '../../core/services/dashboard.service';

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
  imports: [
    MatCardModule,
    MatIconModule,
    MatProgressBarModule,
    MatButtonModule,
    ShiftCardComponent,
    EquityWidgetComponent,
    LeaveSummaryComponent,
  ],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.scss',
})
export class DashboardComponent implements OnInit {
  private http = inject(HttpClient);
  private router = inject(Router);
  auth = inject(AuthService);
  dashboard = inject(DashboardService);

  isEmployee = this.auth.hasRole('Employee');
  kpis = signal<DashboardKpi | null>(null);
  heatmap = signal<HeatmapData | null>(null);
  alerts = signal<Alert[]>([]);

  todayShiftData = this.dashboard.todayShift;
  equityScore = this.dashboard.equityScore;
  equityLabel = this.dashboard.equityLabel;
  leaveBalance = this.dashboard.leaveBalance;
  notificationsUnread = this.dashboard.notificationsUnread;

  defaultShiftCard: ShiftCardData = {
    shiftCode: 'A',
    startTime: '9h00',
    endTime: '16h00',
    cellule: 'Cellule A',
    status: 'En cours',
  };

  ngOnInit(): void {
    if (this.isEmployee) {
      const empId = this.auth.user()?.employeeId ?? this.auth.user()?.id;
      if (empId) this.dashboard.loadEmployeeDashboard(empId);
    } else {
      this.loadManagerDashboard();
    }
  }

  private loadManagerDashboard(): void {
    this.http.get<DashboardKpi>(`${environment.apiUrl}/api/analytics/dashboard/kpis`).subscribe((k) => this.kpis.set(k));
    this.http.get<HeatmapData>(`${environment.apiUrl}/api/analytics/dashboard/heatmap`).subscribe((h) => this.heatmap.set(h));
    this.http.get<Alert[]>(`${environment.apiUrl}/api/analytics/dashboard/alerts`).subscribe((a) => this.alerts.set(a));
  }

  getHeatmapColor(val: number): string {
    if (val === 0) return '#f44336';
    if (val === 1) return '#ff9800';
    return '#4caf50';
  }

  onNewAbsenceRequest(): void {
    this.router.navigate(['/my-leaves/new']);
  }
}
