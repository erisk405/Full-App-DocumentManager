import { provideAnimations } from '@angular/platform-browser/animations';
import { providePrimeNG } from 'primeng/config';
import { ApplicationConfig, provideZoneChangeDetection } from '@angular/core';
import { provideRouter } from '@angular/router';
import { routes } from './app.routes';
import Noir from './mypreset';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { authInterceptor } from './features/core/auth.interceptor';

export const appConfig: ApplicationConfig = {
  providers: [
    providePrimeNG({
      theme: {
        preset: Noir,
        options: {
          darkModeSelector: true
        }
      }
    }),
    provideAnimations(),
    provideHttpClient(withInterceptors([authInterceptor])),
    provideZoneChangeDetection({ eventCoalescing: true }), provideRouter(routes)]
};
