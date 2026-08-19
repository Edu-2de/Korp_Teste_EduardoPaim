import { DatePipe } from '@angular/common';
import { Component, ViewChild, computed, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { QueryClient, injectMutation, injectQuery } from '@tanstack/angular-query-experimental';
import { MessageService } from 'primeng/api';
import { ButtonModule } from 'primeng/button';
import { DialogModule } from 'primeng/dialog';
import { InputTextModule } from 'primeng/inputtext';
import { SelectModule } from 'primeng/select';
import { TableModule } from 'primeng/table';
import { TooltipModule } from 'primeng/tooltip';
import { apiErrorMessage } from '../../../core/http/api-client';
import { Invoice } from '../../../core/models/invoice.model';
import { Product } from '../../../core/models/product.model';
import { InvoiceService } from '../../../core/services/invoice.service';
import { ProductService } from '../../../core/services/product.service';
import { InvoiceItemPicked, InvoiceItemPicker } from '../invoice-item-picker/invoice-item-picker';

type InvoiceStatusFilter = 'all' | 'open' | 'closed';

@Component({
  selector: 'app-invoice-list',
  standalone: true,
  imports: [
    TableModule,
    ButtonModule,
    TooltipModule,
    DialogModule,
    FormsModule,
    SelectModule,
    InputTextModule,
    DatePipe,
    RouterLink,
    InvoiceItemPicker,
  ],
  templateUrl: './invoice-list.html',
  styleUrl: './invoice-list.scss',
})
export class InvoiceList {
  @ViewChild(InvoiceItemPicker) private itemPicker!: InvoiceItemPicker;

  private invoiceService = inject(InvoiceService);
  private productService = inject(ProductService);
  private messageService = inject(MessageService);
  private queryClient = inject(QueryClient);

  invoicesQuery = injectQuery(() => ({
    queryKey: ['invoices'],
    queryFn: () => this.invoiceService.getAll(),
  }));

  productsQuery = injectQuery(() => ({
    queryKey: ['products'],
    queryFn: () => this.productService.getAll(),
  }));

  productById = computed(() => {
    const map = new Map<string, Product>();
    for (const product of this.productsQuery.data() ?? []) {
      map.set(product.id, product);
    }
    return map;
  });

  searchTerm = signal('');

  statusOptions: { label: string; value: InvoiceStatusFilter }[] = [
    { label: 'Todas', value: 'all' },
    { label: 'Abertas', value: 'open' },
    { label: 'Fechadas', value: 'closed' },
  ];
  statusFilter = signal<InvoiceStatusFilter>('all');

  displayedInvoices = computed(() => {
    const term = this.searchTerm().trim().toLowerCase();
    const status = this.statusFilter();
    const products = this.productById();

    return (this.invoicesQuery.data() ?? []).filter((invoice) => {
      const matchesStatus =
        status === 'all' ||
        (status === 'open' ? invoice.status === 'Open' : invoice.status === 'Closed');

      const matchesSearch =
        term === '' ||
        String(invoice.number).includes(term) ||
        invoice.items.some((item) => {
          const product = products.get(item.productId);
          return (
            (product?.description.toLowerCase().includes(term) ?? false) ||
            (product?.code?.toLowerCase().includes(term) ?? false)
          );
        });

      return matchesStatus && matchesSearch;
    });
  });

  editingInvoiceId = signal<string | null>(null);

  printingInvoiceId = signal<string | null>(null);

  deletingInvoiceId = signal<string | null>(null);

  editingInvoice = computed<Invoice | null>(
    () =>
      this.invoicesQuery.data()?.find((invoice) => invoice.id === this.editingInvoiceId()) ?? null,
  );

  printingInvoice = computed<Invoice | null>(
    () =>
      this.invoicesQuery.data()?.find((invoice) => invoice.id === this.printingInvoiceId()) ?? null,
  );

  deletingInvoice = computed<Invoice | null>(
    () =>
      this.invoicesQuery.data()?.find((invoice) => invoice.id === this.deletingInvoiceId()) ?? null,
  );

  addedProductIds = computed(
    () => new Set((this.editingInvoice()?.items ?? []).map((item) => item.productId)),
  );

  availableProducts = computed(() =>
    (this.productsQuery.data() ?? []).filter(
      (product) =>
        product.isActive && product.balance > 0 && !this.addedProductIds().has(product.id),
    ),
  );

  addItemMutation = injectMutation(() => ({
    mutationFn: (dto: InvoiceItemPicked) => {
      const invoiceId = this.editingInvoiceId();
      if (!invoiceId) return Promise.reject(new Error('Nenhuma nota selecionada.'));
      return this.invoiceService.addItem(invoiceId, dto);
    },
    onSuccess: () => {
      this.queryClient.invalidateQueries({ queryKey: ['invoices'] });
      this.itemPicker.reset();
      this.messageService.add({
        severity: 'success',
        summary: 'Item adicionado',
        detail: 'Produto adicionado à nota com sucesso.',
      });
    },
    onError: (err) => {
      this.messageService.add({
        severity: 'error',
        summary: 'Erro ao adicionar item',
        detail: apiErrorMessage(err, 'Tente novamente.'),
      });
    },
  }));

  removeItemMutation = injectMutation(() => ({
    mutationFn: (itemId: string) => {
      const invoiceId = this.editingInvoiceId();
      if (!invoiceId) return Promise.reject(new Error('Nenhuma nota selecionada.'));
      return this.invoiceService.removeItem(invoiceId, itemId);
    },
    onSuccess: () => {
      this.queryClient.invalidateQueries({ queryKey: ['invoices'] });
      this.messageService.add({
        severity: 'success',
        summary: 'Item removido',
        detail: 'O produto foi removido da nota.',
      });
    },
    onError: (err) => {
      this.messageService.add({
        severity: 'error',
        summary: 'Erro ao remover item',
        detail: apiErrorMessage(err, 'Não foi possível remover o item.'),
      });
    },
  }));

  printMutation = injectMutation(() => ({
    mutationFn: () => {
      const invoiceId = this.printingInvoiceId();
      if (!invoiceId) return Promise.reject(new Error('Nenhuma nota selecionada.'));
      return this.invoiceService.print(invoiceId);
    },
    onSuccess: () => {
      this.queryClient.invalidateQueries({ queryKey: ['invoices'] });
      this.queryClient.invalidateQueries({ queryKey: ['products'] });
      this.messageService.add({
        severity: 'success',
        summary: 'Nota emitida',
        detail: 'A nota fiscal foi fechada e o saldo dos produtos foi atualizado.',
      });
      this.closePrint();
    },
    onError: (err) => {
      this.messageService.add({
        severity: 'error',
        summary: 'Falha ao emitir',
        detail: apiErrorMessage(err, 'Não foi possível emitir a nota.'),
      });
    },
  }));

  deleteMutation = injectMutation(() => ({
    mutationFn: () => {
      const invoiceId = this.deletingInvoiceId();
      if (!invoiceId) return Promise.reject(new Error('Nenhuma nota selecionada.'));
      return this.invoiceService.delete(invoiceId);
    },
    onSuccess: () => {
      this.queryClient.invalidateQueries({ queryKey: ['invoices'] });
      this.messageService.add({
        severity: 'success',
        summary: 'Nota excluída',
        detail: 'A nota fiscal foi excluída.',
      });
      this.closeDelete();
    },
    onError: (err) => {
      this.messageService.add({
        severity: 'error',
        summary: 'Falha ao excluir',
        detail: apiErrorMessage(err, 'Não foi possível excluir a nota.'),
      });
    },
  }));

  openEditItems(invoice: Invoice): void {
    this.editingInvoiceId.set(invoice.id);
  }

  closeEditItems(): void {
    this.editingInvoiceId.set(null);
  }

  addItem(picked: InvoiceItemPicked): void {
    this.addItemMutation.mutate(picked);
  }

  removeItem(itemId: string): void {
    this.removeItemMutation.mutate(itemId);
  }

  openPrint(invoice: Invoice): void {
    this.printingInvoiceId.set(invoice.id);
  }

  closePrint(): void {
    this.printingInvoiceId.set(null);
  }

  confirmPrint(): void {
    this.printMutation.mutate();
  }

  openDelete(invoice: Invoice): void {
    this.deletingInvoiceId.set(invoice.id);
  }

  closeDelete(): void {
    this.deletingInvoiceId.set(null);
  }

  confirmDelete(): void {
    this.deleteMutation.mutate();
  }

  totalUnits(items: { quantity: number }[]): number {
    return items.reduce((sum, item) => sum + item.quantity, 0);
  }
}
