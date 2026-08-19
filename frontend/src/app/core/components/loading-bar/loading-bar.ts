import { ChangeDetectionStrategy, Component, computed, inject } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import {
  NavigationCancel,
  NavigationEnd,
  NavigationError,
  NavigationStart,
  Router,
} from '@angular/router';
import { injectIsFetching, injectIsMutating } from '@tanstack/angular-query-experimental';
import { filter, map } from 'rxjs';

@Component({
  selector: 'app-loading-bar',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    @if (isLoading()) {
      <div class="loading-bar" role="status" aria-live="polite" aria-label="Carregando"></div>
    }
  `,
  styles: `
    .loading-bar {
      position: fixed;
      inset-inline: 0;
      top: 0;
      z-index: 100;
      height: 3px;
      overflow: hidden;
      background: var(--color-primary-100);
    }

    .loading-bar::after {
      content: '';
      position: absolute;
      inset-block: 0;
      width: 40%;
      border-radius: 999px;
      background: var(--color-primary-600);
      animation: loading-bar-sweep 1.1s ease-in-out infinite;
    }

    @keyframes loading-bar-sweep {
      0% {
        transform: translateX(-100%);
      }
      100% {
        transform: translateX(350%);
      }
    }

    @media (prefers-reduced-motion: reduce) {
      .loading-bar::after {
        animation: none;
        width: 100%;
        transform: none;
      }
    }
  `,
})
export class LoadingBar {
  private router = inject(Router);

  private isNavigating = toSignal(
    this.router.events.pipe(
      filter(
        (event): event is NavigationStart | NavigationEnd | NavigationCancel | NavigationError =>
          event instanceof NavigationStart ||
          event instanceof NavigationEnd ||
          event instanceof NavigationCancel ||
          event instanceof NavigationError,
      ),
      map((event) => event instanceof NavigationStart),
    ),
    { initialValue: false },
  );

  private isFetching = injectIsFetching();
  private isMutating = injectIsMutating();

  isLoading = computed(() => this.isNavigating() || this.isFetching() > 0 || this.isMutating() > 0);
}
