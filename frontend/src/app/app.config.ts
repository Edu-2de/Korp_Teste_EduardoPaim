import { ApplicationConfig } from '@angular/core';
import { provideAnimationsAsync } from '@angular/platform-browser/animations/async';
import { provideRouter, withViewTransitions } from '@angular/router';
import Aura from '@primeng/themes/aura';
import { definePreset } from '@primeng/themes';
import { MessageService } from 'primeng/api';
import { providePrimeNG } from 'primeng/config';
import { QueryClient, provideTanStackQuery } from '@tanstack/angular-query-experimental';
import { routes } from './app.routes';

const AppTheme = definePreset(Aura, {
  primitive: {
    borderRadius: {
      xs: '4px',
      sm: '6px',
      md: '10px',
      lg: '12px',
      xl: '16px',
    },
  },
  semantic: {
    primary: {
      50: '#fef1f4',
      100: '#fcdee5',
      200: '#ffb8c9',
      300: '#ff809e',
      400: '#ff426f',
      500: '#ff265a',
      600: '#ff0c46',
      700: '#e00035',
      800: '#b8002c',
      900: '#8a0021',
      950: '#570015',
    },
    colorScheme: {
      light: {
        surface: {
          50: '#f6f8f8',
          100: '#edf0f2',
          200: '#dae2e7',
          300: '#bcc9d2',
          400: '#91a7b6',
          500: '#668599',
          600: '#4e6574',
          700: '#375467',
          800: '#2b485a',
          900: '#1e2e38',
          950: '#121b21',
        },
      },
    },
  },
  components: {
    tag: {
      colorScheme: {
        light: {
          success: { background: '{teal.100}', color: '{teal.700}' },
          info: { background: '#dfecf6', color: '#114b78' },
          warn: { background: '{amber.100}', color: '{amber.700}' },
          danger: { background: '{red.100}', color: '{red.700}' },
        },
      },
    },
    toast: {
      colorScheme: {
        light: {
          success: {
            background: 'color-mix(in srgb, {teal.50}, transparent 5%)',
            borderColor: '{teal.200}',
            color: '{teal.600}',
          },
          error: {
            background: 'color-mix(in srgb, {red.50}, transparent 5%)',
            borderColor: '{red.200}',
            color: '{red.600}',
          },
        },
      },
    },
  },
});

export const appConfig: ApplicationConfig = {
  providers: [
    provideRouter(routes, withViewTransitions()),
    provideAnimationsAsync(),
    providePrimeNG({
      theme: {
        preset: AppTheme,
        options: { darkModeSelector: false },
      },
    }),
    provideTanStackQuery(
      new QueryClient({
        defaultOptions: {
          queries: {
            retry: 1,
            refetchOnWindowFocus: false,
          },
        },
      }),
    ),
    MessageService,
  ],
};
