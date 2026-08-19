import { Routes } from '@angular/router';

export const routes: Routes = [
  { path: '', redirectTo: 'products', pathMatch: 'full' },
  {
    path: 'products',
    loadComponent: () => import('./features/products/product-list/product-list').then((m) => m.ProductList),
    title: 'Produtos',
  },
  {
    path: 'products/new',
    loadComponent: () =>
      import('./features/products/product-create/product-create').then((m) => m.ProductCreate),
    title: 'Novo Produto',
  },
  {
    path: 'invoices',
    loadComponent: () =>
      import('./features/invoices/invoice-list/invoice-list').then((m) => m.InvoiceList),
    title: 'Notas Fiscais',
  },
  {
    path: 'invoices/new',
    loadComponent: () =>
      import('./features/invoices/invoice-create/invoice-create').then((m) => m.InvoiceCreate),
    title: 'Nova Nota Fiscal',
  },
  { path: '**', redirectTo: 'products' },
];
