import { Component, ViewChild, computed, inject, signal } from '@angular/core';
import { Router } from '@angular/router';
import { QueryClient, injectMutation, injectQuery } from '@tanstack/angular-query-experimental';
import { MessageService } from 'primeng/api';
import { ButtonModule } from 'primeng/button';
import { apiErrorMessage } from '../../../core/http/api-client';
import { Product } from '../../../core/models/product.model';
import { InvoiceService } from '../../../core/services/invoice.service';
import { ProductService } from '../../../core/services/product.service';
import { InvoiceItemPicked, InvoiceItemPicker } from '../invoice-item-picker/invoice-item-picker';

interface DraftItem {
  product: Product;
  quantity: number;
}

@Component({
  selector: 'app-invoice-create',
  standalone: true,
  imports: [ButtonModule, InvoiceItemPicker],
  templateUrl: './invoice-create.html',
  styleUrl: './invoice-create.scss',
})
export class InvoiceCreate {
  @ViewChild(InvoiceItemPicker) private itemPicker!: InvoiceItemPicker;

  private invoiceService = inject(InvoiceService);
  private productService = inject(ProductService);
  private router = inject(Router);
  private messageService = inject(MessageService);
  private queryClient = inject(QueryClient);

  productsQuery = injectQuery(() => ({
    queryKey: ['products'],
    queryFn: () => this.productService.getAll(),
  }));

  draftItems = signal<DraftItem[]>([]);

  private draftedProductIds = computed(() => new Set(this.draftItems().map((item) => item.product.id)));

  availableProducts = computed(() =>
    (this.productsQuery.data() ?? []).filter(
      (product) => product.isActive && product.balance > 0 && !this.draftedProductIds().has(product.id),
    ),
  );

  totalUnits = computed(() => this.draftItems().reduce((sum, item) => sum + item.quantity, 0));

  /** Cria a nota e, em seguida, lança nela cada item do rascunho — só existe no backend a partir daqui. */
  createMutation = injectMutation(() => ({
    mutationFn: async () => {
      const invoice = await this.invoiceService.create();
      for (const draft of this.draftItems()) {
        await this.invoiceService.addItem(invoice.id, { productId: draft.product.id, quantity: draft.quantity });
      }
      return invoice;
    },
    onSuccess: () => {
      this.queryClient.invalidateQueries({ queryKey: ['invoices'] });
      this.messageService.add({
        severity: 'success',
        summary: 'Nota criada',
        detail: 'A nota fiscal foi criada com os itens selecionados.',
      });
      this.draftItems.set([]);
    },
    onError: (err) => {
      // A nota pode ter sido criada mesmo se um item específico falhou —
      // invalida a lista para ela aparecer e poder ser completada depois.
      this.queryClient.invalidateQueries({ queryKey: ['invoices'] });
      this.messageService.add({
        severity: 'error',
        summary: 'Erro ao criar nota',
        detail: apiErrorMessage(err, 'Não foi possível criar a nota fiscal.'),
      });
    },
  }));

  addDraftItem(picked: InvoiceItemPicked): void {
    const product = this.productsQuery.data()?.find((p) => p.id === picked.productId);
    if (!product) return;
    this.draftItems.update((items) => [...items, { product, quantity: picked.quantity }]);
    this.itemPicker.reset();
  }

  removeDraftItem(productId: string): void {
    this.draftItems.update((items) => items.filter((item) => item.product.id !== productId));
  }

  submitCreate(): void {
    if (this.draftItems().length === 0) return;
    this.createMutation.mutate();
  }

  goBack(): void {
    this.router.navigate(['/invoices']);
  }
}
