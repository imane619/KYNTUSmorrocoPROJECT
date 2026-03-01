import { MenuItem } from '../../shared/models/menu-item';

export const MANAGER_MENU: MenuItem[] = [
  { label: 'Accueil', icon: 'home', route: '/', roles: ['Admin', 'Manager'] },
  { label: 'Employés', icon: 'people', route: '/employees', roles: ['Admin', 'Manager'] },
  { label: 'Planning', icon: 'calendar_month', route: '/planning', roles: ['Admin', 'Manager'] },
  { label: 'Congés', icon: 'work', route: '/leaves', roles: ['Admin', 'Manager'] },
  { label: 'Analytics', icon: 'bar_chart', route: '/analytics', roles: ['Admin', 'Manager'] },
  { label: 'Reporting', icon: 'description', route: '/reporting', roles: ['Admin', 'Manager'] },
  { label: 'Notifications', icon: 'notifications', route: '/notifications', roles: ['Admin', 'Manager'] },
  { label: 'Paramètres', icon: 'settings', route: '/settings', roles: ['Admin', 'Manager'] }
];

export const EMPLOYEE_MENU: MenuItem[] = [
  { label: 'Accueil', icon: 'home', route: '/', roles: ['Employee'] },
  { label: 'Mon Planning', icon: 'calendar_month', route: '/my-planning', roles: ['Employee'] },
  { label: 'Mes Congés', icon: 'palm_tree', route: '/my-leaves', roles: ['Employee'] },
  { label: 'Notifications', icon: 'notifications', route: '/notifications', roles: ['Employee'] },
  { label: 'Mon Score Équité', icon: 'balance', route: '/my-equity', roles: ['Employee'] }
];
