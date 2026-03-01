import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth.guard';
import { roleGuard } from './core/guards/role.guard';
import { LayoutComponent } from './layout/layout.component';

export const routes: Routes = [
  { path: 'login', loadComponent: () => import('./pages/login/login.component').then((m) => m.LoginComponent) },
  {
    path: '',
    component: LayoutComponent,
    canActivate: [authGuard],
    children: [
      { path: '', loadComponent: () => import('./pages/dashboard/dashboard.component').then((m) => m.DashboardComponent) },
      {
        path: 'employees',
        loadComponent: () => import('./pages/employees/employees.component').then((m) => m.EmployeesComponent),
        canActivate: [roleGuard(['Admin', 'Manager'])]
      },
      {
        path: 'planning',
        loadComponent: () => import('./pages/planning/planning.component').then((m) => m.PlanningComponent),
        canActivate: [roleGuard(['Admin', 'Manager'])]
      },

      {
        path: 'analytics',
        loadComponent: () => import('./pages/analytics/analytics.component').then((m) => m.AnalyticsComponent),
        canActivate: [roleGuard(['Admin', 'Manager'])]
      },
      {
        path: 'reporting',
        loadComponent: () => import('./pages/reporting/reporting.component').then((m) => m.ReportingComponent),
        canActivate: [roleGuard(['Admin', 'Manager'])]
      },
      { path: 'notifications', loadComponent: () => import('./pages/notifications/notifications.component').then((m) => m.NotificationsComponent) },
      { path: 'settings', loadComponent: () => import('./pages/settings/settings.component').then((m) => m.SettingsComponent) },
      { path: 'my-planning', loadComponent: () => import('./pages/my-planning/my-planning.component').then((m) => m.MyPlanningComponent), canActivate: [roleGuard(['Employee'])] },
      { path: 'my-leaves', loadComponent: () => import('./pages/my-leaves/my-leaves.component').then((m) => m.MyLeavesComponent), canActivate: [roleGuard(['Employee'])] },
      { path: 'my-leaves/new', loadComponent: () => import('./pages/my-leaves/absence-form/absence-form.component').then((m) => m.AbsenceFormComponent),canActivate: [roleGuard(['Employee'])] 
      },
      { path: 'my-equity', loadComponent: () => import('./pages/my-equity/my-equity.component').then((m) => m.MyEquityComponent), canActivate: [roleGuard(['Employee'])] }
    ]
  },
  { path: '**', redirectTo: '' }
];
