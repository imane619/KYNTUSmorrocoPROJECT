import { Injectable, signal, computed } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { Observable, tap } from 'rxjs';
import { environment } from '../../../environments/environment';

export interface UserInfo {
  id: string;
  email: string;
  firstName: string;
  lastName: string;
  fullName: string;
  role: string;
  employeeId?: string;
}

export interface LoginResponse {
  token: string;
  refreshToken: string;
  expiresAt: string;
  user: UserInfo;
}

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly tokenKey = 'shiftmaster_token';
  private readonly userKey = 'shiftmaster_user';

  private tokenSignal = signal<string | null>(this.getStoredToken());
  private userSignal = signal<UserInfo | null>(this.getStoredUser());

  token = this.tokenSignal.asReadonly();
  user = this.userSignal.asReadonly();
  isAuthenticated = computed(() => !!this.tokenSignal());

  constructor(
    private http: HttpClient,
    private router: Router
  ) {}

login(email: string, password: string): Observable<LoginResponse> {
  return this.http
    .post<LoginResponse>(`${environment.apiUrl}/Auth/login`, { email, password })
    .pipe(
      tap((res) => {
        this.tokenSignal.set(res.token);
        this.userSignal.set(res.user);
        localStorage.setItem(this.tokenKey, res.token);
        localStorage.setItem(this.userKey, JSON.stringify(res.user));
      })
    );
}

  logout(): void {
    this.tokenSignal.set(null);
    this.userSignal.set(null);
    localStorage.removeItem(this.tokenKey);
    localStorage.removeItem(this.userKey);
    this.router.navigate(['/login']);
  }

  getToken(): string | null {
    return this.tokenSignal();
  }

  hasRole(role: string): boolean {
    const user = this.userSignal();
    return user?.role === role;
  }

  hasAnyRole(roles: string[]): boolean {
    const user = this.userSignal();
    return user ? roles.includes(user.role) : false;
  }

  private getStoredToken(): string | null {
    return localStorage.getItem(this.tokenKey);
  }

  private getStoredUser(): UserInfo | null {
    const stored = localStorage.getItem(this.userKey);
    return stored ? JSON.parse(stored) : null;
  }
}
