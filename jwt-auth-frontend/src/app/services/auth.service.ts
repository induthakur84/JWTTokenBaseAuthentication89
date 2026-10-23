import { Injectable, signal, computed, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { Observable, tap } from 'rxjs';
import { LoginRequest, LoginResponse, RegisterRequest, UserResponse, JwtDecodedToken } from '../models/auth.models';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private http = inject(HttpClient);
  private router = inject(Router);

  // Storage Keys
  private readonly TOKEN_KEY = 'jwt_token';
  private readonly USER_KEY = 'current_user';
  private readonly API_URL_KEY = 'api_base_url';

  // Base API URL: default to HTTPS (7138) or fallback to HTTP (5073)
  private readonly defaultApiUrl = 'https://localhost:7138/api';
  readonly apiUrl = signal<string>(localStorage.getItem(this.API_URL_KEY) || this.defaultApiUrl);

  // Reactive State Signals (Angular Modern State Management)
  readonly currentUser = signal<UserResponse | null>(this.getStoredUser());
  readonly token = signal<string | null>(this.getStoredToken());

  // Computed signals
  readonly isLoggedIn = computed(() => !!this.token());
  readonly userRole = computed(() => this.currentUser()?.role || '');

  constructor() {}

  /**
   * Update the backend URL (allows students to switch between HTTPS 7138 and HTTP 5073)
   */
  setApiUrl(url: string): void {
    const cleanUrl = url.replace(/\/+$/, '');
    localStorage.setItem(this.API_URL_KEY, cleanUrl);
    this.apiUrl.set(cleanUrl);
  }

  /**
   * 1. LOGIN API CALL
   * Sends { username, password } to /api/User/Login
   * Stores received JWT token and user info upon success
   */
  login(request: LoginRequest): Observable<LoginResponse> {
    return this.http.post<LoginResponse>(`${this.apiUrl()}/User/Login`, request).pipe(
      tap((response) => {
        if (response && response.token) {
          // Store token and user in localStorage
          localStorage.setItem(this.TOKEN_KEY, response.token);
          localStorage.setItem(this.USER_KEY, JSON.stringify(response.user));

          // Update reactive signals
          this.token.set(response.token);
          this.currentUser.set(response.user);
        }
      })
    );
  }

  /**
   * 2. REGISTER API CALL
   * Sends { firstName, lastName, username, password, confirmPassword, role } to /api/User/Register
   */
  register(request: RegisterRequest): Observable<any> {
    return this.http.post<any>(`${this.apiUrl()}/User/Register`, request);
  }

  /**
   * 3. LOGOUT
   * Clears token and user info, updates reactive state, and redirects to login
   */
  logout(): void {
    localStorage.removeItem(this.TOKEN_KEY);
    localStorage.removeItem(this.USER_KEY);
    this.token.set(null);
    this.currentUser.set(null);
    this.router.navigate(['/login']);
  }

  /**
   * Get raw token string
   */
  getToken(): string | null {
    return localStorage.getItem(this.TOKEN_KEY);
  }

  /**
   * Check if token exists and is not expired
   */
  isAuthenticated(): boolean {
    const token = this.getToken();
    if (!token) return false;
    return !this.isTokenExpired(token);
  }

  /**
   * Check if the logged in user has a specific role
   */
  hasRole(role: string): boolean {
    return this.userRole().toLowerCase() === role.toLowerCase();
  }

  /**
   * Helper to decode the JWT string into Header, Payload, and Signature.
   * Educational visualizer for students: explains what is inside Base64url encoded parts!
   */
  decodeToken(tokenStr?: string): JwtDecodedToken | null {
    const token = tokenStr || this.getToken();
    if (!token) return null;

    try {
      const parts = token.split('.');
      if (parts.length !== 3) return null;

      const decodeBase64Url = (str: string) => {
        let base64 = str.replace(/-/g, '+').replace(/_/g, '/');
        while (base64.length % 4) {
          base64 += '=';
        }
        return decodeURIComponent(
          atob(base64)
            .split('')
            .map((c) => '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2))
            .join('')
        );
      };

      const header = JSON.parse(decodeBase64Url(parts[0]));
      const payload = JSON.parse(decodeBase64Url(parts[1]));
      const signature = parts[2];

      return { header, payload, signature };
    } catch (e) {
      console.error('Error decoding JWT token', e);
      return null;
    }
  }

  /**
   * Check if JWT token is expired based on the 'exp' claim (epoch seconds)
   */
  isTokenExpired(tokenStr?: string): boolean {
    const decoded = this.decodeToken(tokenStr);
    if (!decoded || !decoded.payload.exp) return false;
    const nowInSeconds = Math.floor(Date.now() / 1000);
    return decoded.payload.exp < nowInSeconds;
  }

  private getStoredToken(): string | null {
    return localStorage.getItem(this.TOKEN_KEY);
  }

  private getStoredUser(): UserResponse | null {
    const stored = localStorage.getItem(this.USER_KEY);
    if (!stored) return null;
    try {
      return JSON.parse(stored);
    } catch {
      return null;
    }
  }
}
