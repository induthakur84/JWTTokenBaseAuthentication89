import { inject } from '@angular/core';
import { CanActivateFn, Router, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { AuthService } from '../services/auth.service';

/**
 * AUTH GUARD (CanActivateFn in modern Angular)
 * --------------------------------------------
 * Protects routes from unauthorized access.
 * If user has a valid JWT token, allow access.
 * If not, redirect to /login and preserve destination URL.
 */
export const authGuard: CanActivateFn = (
  route: ActivatedRouteSnapshot,
  state: RouterStateSnapshot
) => {
  const authService = inject(AuthService);
  const router = inject(Router);

  if (authService.isAuthenticated()) {
    return true;
  }

  // Not authenticated: redirect to login page with return url
  return router.createUrlTree(['/login'], {
    queryParams: { returnUrl: state.url }
  });
};

/**
 * ROLE GUARD
 * Checks if user is logged in AND has one of the required roles.
 */
export const roleGuard = (expectedRoles: string[]): CanActivateFn => {
  return () => {
    const authService = inject(AuthService);
    const router = inject(Router);

    if (!authService.isAuthenticated()) {
      return router.createUrlTree(['/login']);
    }

    const currentRole = authService.userRole().toLowerCase();
    const hasRole = expectedRoles.some((r) => r.toLowerCase() === currentRole);

    if (hasRole) {
      return true;
    }

    // Role unauthorized
    alert(`Access Denied! Your role (${authService.userRole()}) cannot access this page.`);
    return router.createUrlTree(['/dashboard']);
  };
};
