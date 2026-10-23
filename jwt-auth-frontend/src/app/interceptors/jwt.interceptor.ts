import { HttpInterceptorFn, HttpErrorResponse } from '@angular/common/http';
import { inject } from '@angular/core';
import { AuthService } from '../services/auth.service';
import { catchError, throwError } from 'rxjs';

/**
 * JWT HTTP INTERCEPTOR (Functional Interceptor in modern Angular)
 * -------------------------------------------------------------
 * Why do we need this?
 * 1. Automatically intercepts every outgoing HTTP request from Angular.
 * 2. Checks if a JWT token is stored in localStorage.
 * 3. If present, attaches the header: "Authorization: Bearer <token>"
 * 4. Listens for 401 Unauthorized errors (e.g., token expired or invalid)
 *    and triggers logout/redirect automatically.
 */
export const jwtInterceptor: HttpInterceptorFn = (req, next) => {
  const authService = inject(AuthService);
  const token = authService.getToken();

  // Clone request to add the Bearer token in the Authorization header
  let authReq = req;
  if (token) {
    authReq = req.clone({
      setHeaders: {
        Authorization: `Bearer ${token}`
      }
    });
  }

  // Pass cloned request to the next handler and intercept errors
  return next(authReq).pipe(
    catchError((error: HttpErrorResponse) => {
      // If backend returns 401 Unauthorized (e.g. invalid or expired JWT token)
      if (error.status === 401) {
        console.warn('[JwtInterceptor] 401 Unauthorized received - Token invalid or expired.');
        authService.logout();
      }
      return throwError(() => error);
    })
  );
};
