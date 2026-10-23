import { Component, inject, OnInit, OnDestroy } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { JsonPipe } from '@angular/common';
import { AuthService } from '../../services/auth.service';
import { JwtDecodedToken } from '../../models/auth.models';

@Component({
  selector: 'app-dashboard',
  imports: [RouterLink, JsonPipe],  
  templateUrl: './dashboard.component.html'
})
export class DashboardComponent implements OnInit, OnDestroy {
  authService = inject(AuthService);
  private router = inject(Router);

  rawToken: string | null = null;
  tokenParts: { header: string; payload: string; signature: string } | null = null;
  decoded: JwtDecodedToken | null = null;

  timeRemaining = '';
  isExpired = false;
  copySuccess = false;
  tamperMessage = '';
  private timerInterval: any;

  ngOnInit(): void {
    this.refreshJwtData();

    // Update expiry timer every second
    this.timerInterval = setInterval(() => {
      this.updateExpiryTimer();
    }, 1000);
  }

  ngOnDestroy(): void {
    if (this.timerInterval) {
      clearInterval(this.timerInterval);
    }
  }

  refreshJwtData(): void {
    this.rawToken = this.authService.getToken();
    if (this.rawToken) {
      const parts = this.rawToken.split('.');
      if (parts.length === 3) {
        this.tokenParts = {
          header: parts[0],
          payload: parts[1],
          signature: parts[2]
        };
      }
      this.decoded = this.authService.decodeToken(this.rawToken);
      this.updateExpiryTimer();
    }
  }

  updateExpiryTimer(): void {
    if (!this.decoded || !this.decoded.payload.exp) {
      this.timeRemaining = 'No expiry claim found';
      return;
    }

    const expTime = this.decoded.payload.exp * 1000;
    const now = Date.now();
    const diff = expTime - now;

    if (diff <= 0) {
      this.timeRemaining = 'EXPIRED';
      this.isExpired = true;
    } else {
      this.isExpired = false;
      const minutes = Math.floor(diff / 60000);
      const seconds = Math.floor((diff % 60000) / 1000);
      this.timeRemaining = `${minutes}m ${seconds}s`;
    }
  }

  copyToken(): void {
    if (this.rawToken) {
      navigator.clipboard.writeText(this.rawToken).then(() => {
        this.copySuccess = true;
        setTimeout(() => (this.copySuccess = false), 2500);
      });
    }
  }

  tamperToken(): void {
    if (!this.rawToken) return;

    // Tamper with the signature by altering the last 3 characters
    const tampered = this.rawToken.substring(0, this.rawToken.length - 3) + 'XYZ';
    localStorage.setItem('jwt_token', tampered);
    this.authService.token.set(tampered);
    this.rawToken = tampered;
    this.refreshJwtData();

    this.tamperMessage =
      '⚠️ Token signature tampered! Now go to "Protected API" and make a request to see the .NET backend reject it with 401 Unauthorized!';
  }

  formatDate(epochSeconds?: number): string {
    if (!epochSeconds) return 'N/A';
    return new Date(epochSeconds * 1000).toLocaleTimeString([], {
      hour: '2-digit',
      minute: '2-digit',
      second: '2-digit'
    });
  }

  logout(): void {
    this.authService.logout();
  }
}
