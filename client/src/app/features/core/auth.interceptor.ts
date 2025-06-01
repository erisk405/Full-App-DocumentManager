import { HttpErrorResponse, HttpHandler, HttpInterceptorFn, HttpRequest } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { Observable, catchError, throwError } from 'rxjs';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const router = inject(Router);
  const token = localStorage.getItem('token');
  
  // Skip adding token for login and public endpoints
  const isPublicEndpoint = req.url.includes('/login') || req.url.includes('/register');
  
  if (token && !isPublicEndpoint) {
    const authReq = req.clone({
      setHeaders: {
        Authorization: `Bearer ${token}`
      }
    });
    
    return next(authReq).pipe(
      catchError((error: HttpErrorResponse) => {
        // Handle authentication errors (401 Unauthorized, 403 Forbidden)
        if (error.status === 401 || error.status === 403) {
          console.error('Authentication error:', error.message);
          // Clear token and redirect to login
          localStorage.removeItem('token');
          router.navigate(['/login']);
        }
        return throwError(() => error);
      })
    );
  }
  
  return next(req);
};
