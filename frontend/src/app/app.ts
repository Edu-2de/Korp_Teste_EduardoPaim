import { Component } from '@angular/core';
import { RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { ToastModule } from 'primeng/toast';
import { LoadingBar } from './core/components/loading-bar/loading-bar';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, RouterLink, RouterLinkActive, ToastModule, LoadingBar],
  templateUrl: './app.html',
  styleUrl: './app.scss',
})
export class App {
  readonly navLinks = [
    { path: '/products', label: 'Produtos', icon: 'pi-box' },
    { path: '/invoices', label: 'Notas Fiscais', icon: 'pi-file' },
  ];
}
