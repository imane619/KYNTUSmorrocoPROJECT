import { Injectable, inject, signal, computed } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { AuthService } from './auth.service';

export interface EquityResult {
  employeeId: string;
  score: number;
  shiftsAssigned: number;
  saturdayShifts: number;
  totalHours: number;
  teamAverageHours: number;
  label: string;
}

export interface PlanningEntryDto {
  id: string;
  employeeId: string;
  employeeName: string;
  shiftId: string;
  shiftCode: string;
  date: string;
  startTime?: string;
  endTime?: string;
  status: string;
}

export interface ShiftCardData {
  shiftCode: string;
  startTime: string;
  endTime: string;
  cellule?: string;
  status: 'En cours' | 'Terminé' | 'À venir';
}

export interface AbsenceDto {
  id: string;
  employeeId: string;
  type: string;
  startDate: string;
  endDate: string;
  status: string;
  reason?: string;
}

@Injectable({ providedIn: 'root' })
export class DashboardService {
  private http = inject(HttpClient);
  private auth = inject(AuthService);

  private _todayShift = signal<ShiftCardData | null>(null);
  private _equity = signal<EquityResult | null>(null);
  private _leaveBalance = signal<{ remaining: number; total: number; nextLeave?: string }>({ remaining: 12.5, total: 15 });
  private _notificationsUnread = signal(3);

  todayShift = this._todayShift.asReadonly();
  equity = this._equity.asReadonly();
  leaveBalance = this._leaveBalance.asReadonly();
  notificationsUnread = this._notificationsUnread.asReadonly();

  equityScore = computed(() => this._equity()?.score ?? 85);
  equityLabel = computed(() => this._equity()?.label ?? 'Très bien');

  loadEmployeeDashboard(employeeId: string): void {
    const today = new Date().toISOString().split('T')[0];
    this.http.get<PlanningEntryDto[]>(`${environment.apiUrl}/api/planning?startDate=${today}&endDate=${today}`)
      .subscribe({
        next: entries => {
          const mine = entries.find(e => e.employeeId === employeeId);
          if (mine) {
            const start = mine.startTime ? formatTime(mine.startTime) : '9h00';
            const end = mine.endTime ? formatTime(mine.endTime) : '16h00';
this._todayShift.set({
            shiftCode: mine.shiftCode,
            startTime: start,
            endTime: end,
            cellule: 'Cellule A',
            status: 'En cours',
          });
          } else {
            this._todayShift.set(null);
          }
        },
        error: () => this._todayShift.set(null),
      });

    this.http.get<EquityResult>(`${environment.apiUrl}/api/equity/${employeeId}`)
      .subscribe({ next: e => this._equity.set(e) });

    this.http.get<AbsenceDto[]>(`${environment.apiUrl}/api/absences?employeeId=${employeeId}`)
      .subscribe({
        next: absences => {
          const approved = absences.filter(a => a.status === 'Approved' && a.type === 'PaidLeave');
          const total = 15;
          const used = approved.reduce((sum, a) => {
            const d1 = new Date(a.startDate).getTime();
            const d2 = new Date(a.endDate).getTime();
            return sum + Math.ceil((d2 - d1) / (1000 * 60 * 60 * 24)) + 1;
          }, 0);
          const next = approved
            .filter(a => new Date(a.endDate) >= new Date())
            .sort((a, b) => new Date(a.startDate).getTime() - new Date(b.startDate).getTime())[0];
          this._leaveBalance.set({
            remaining: Math.max(0, total - used),
            total,
            nextLeave: next ? `${formatDate(next.startDate)}-${formatDate(next.endDate)}` : undefined,
          });
        },
      });

    this.http.get<{ read?: boolean }[]>(`${environment.apiUrl}/api/notifications`)
      .subscribe({ next: list => this._notificationsUnread.set(list.filter(n => !n.read).length) });
  }

  markNotificationRead(): void {
    this._notificationsUnread.update(n => Math.max(0, n - 1));
  }

  refresh(): void {
    const user = this.auth.user();
    if (user?.employeeId) this.loadEmployeeDashboard(user.employeeId);
  }
}

function formatTime(t: string): string {
  if (!t) return '';
  const [h, m] = t.split(':');
  return `${parseInt(h, 10)}h${m !== '00' ? m : '00'}`;
}
function formatDate(s: string): string {
  const d = new Date(s);
  return `${d.getDate()} ${['Jan','Fév','Mar','Avr','Mai','Juin','Juil','Août','Sep','Oct','Nov','Déc'][d.getMonth()]}`;
}
