import { Component, computed, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { FormBuilder, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { QueryClient, injectMutation, injectQuery } from '@tanstack/angular-query-experimental';
import { MessageService } from 'primeng/api';
import { ButtonModule } from 'primeng/button';
import { DialogModule } from 'primeng/dialog';
import { InputNumberModule } from 'primeng/inputnumber';
import { InputTextModule } from 'primeng/inputtext';
import { SelectModule } from 'primeng/select';
import { TableModule } from 'primeng/table';
import { TooltipModule } from 'primeng/tooltip';
import { apiErrorMessage } from '../../../core/http/api-client';
import { Product } from '../../../core/models/product.model';
import { ProductService } from '../../../core/services/product.service';

interface ProductEdit {
  description: string;
  balance: number;
}

type StatusFilter = 'active' | 'inactive' | 'all';
type StockFilter = 'all' | 'inStock' | 'outOfStock';

@Component({
  selector: 'app-product-list',
  standalone: true,
  imports: [
    TableModule,
    ButtonModule,
    RouterLink,
    DialogModule,
    ReactiveFormsModule,
    FormsModule,
    InputTextModule,
    InputNumberModule,
    SelectModule,
    TooltipModule,
  ],
  templateUrl: './product-list.html',
  styleUrl: './product-list.scss',
})
export class ProductList {
  private productService = inject(ProductService);
  private queryClient = inject(QueryClient);
  private fb = inject(FormBuilder);
  private messageService = inject(MessageService);

  productsQuery = injectQuery(() => ({
    queryKey: ['products'],
    queryFn: () => this.productService.getAll(),
  }));

  searchTerm = signal('');

  statusOptions: { label: string; value: StatusFilter }[] = [
    { label: 'Ativos', value: 'active' },
    { label: 'Inativos', value: 'inactive' },
    { label: 'Todos', value: 'all' },
  ];
  statusFilter = signal<StatusFilter>('active');

  stockOptions: { label: string; value: StockFilter }[] = [
    { label: 'Qualquer saldo', value: 'all' },
    { label: 'Em estoque', value: 'inStock' },
    { label: 'Sem estoque', value: 'outOfStock' },
  ];
  stockFilter = signal<StockFilter>('all');

  displayedProducts = computed(() => {
    const term = this.searchTerm().trim().toLowerCase();
    const status = this.statusFilter();
    const stock = this.stockFilter();

    return (this.productsQuery.data() ?? []).filter((product) => {
      const matchesSearch =
        term === '' ||
        product.description.toLowerCase().includes(term) ||
        (product.code?.toLowerCase().includes(term) ?? false);
      const matchesStatus =
        status === 'all' || (status === 'active' ? product.isActive : !product.isActive);
      const matchesStock =
        stock === 'all' || (stock === 'inStock' ? product.balance > 0 : product.balance === 0);

      return matchesSearch && matchesStatus && matchesStock;
    });
  });

  editingProduct = signal<Product | null>(null);

  deactivatingProduct = signal<Product | null>(null);

  editForm = this.fb.group({
    description: ['', Validators.required],
    balance: [0, [Validators.required, Validators.min(0)]],
  });

  editMutation = injectMutation(() => ({
    mutationFn: (edit: ProductEdit) => {
      const product = this.editingProduct();
      if (!product) return Promise.reject(new Error('Nenhum produto selecionado.'));
      return Promise.all([
        this.productService.updateDescription(product.id, edit.description),
        this.productService.updateBalance(product.id, edit.balance),
      ]);
    },
    onSuccess: () => {
      this.queryClient.invalidateQueries({ queryKey: ['products'] });
      this.messageService.add({
        severity: 'success',
        summary: 'Produto atualizado',
        detail: 'As alterações foram salvas com sucesso.',
      });
      this.closeEdit();
    },
    onError: (err) => {
      this.messageService.add({
        severity: 'error',
        summary: 'Erro ao atualizar produto',
        detail: apiErrorMessage(err, 'Não foi possível atualizar o produto.'),
      });
    },
  }));

  deactivateMutation = injectMutation(() => ({
    mutationFn: () => {
      const product = this.deactivatingProduct();
      if (!product) return Promise.reject(new Error('Nenhum produto selecionado.'));
      return this.productService.deactivate(product.id);
    },
    onSuccess: () => {
      this.queryClient.invalidateQueries({ queryKey: ['products'] });
      this.messageService.add({
        severity: 'success',
        summary: 'Produto excluído',
        detail: 'O produto foi removido do catálogo.',
      });
      this.closeDeactivate();
    },
    onError: (err) => {
      this.messageService.add({
        severity: 'error',
        summary: 'Erro ao excluir produto',
        detail: apiErrorMessage(err, 'Não foi possível excluir o produto.'),
      });
    },
  }));

  initials(product: Product): string {
    const source = product.code?.trim() || product.description;
    return source.slice(0, 2).toUpperCase();
  }

  openEdit(product: Product): void {
    this.editingProduct.set(product);
    this.editForm.setValue({ description: product.description, balance: product.balance });
  }

  closeEdit(): void {
    this.editingProduct.set(null);
  }

  saveEdit(): void {
    if (this.editForm.invalid) return;
    const { description, balance } = this.editForm.value;
    this.editMutation.mutate({ description: description!.trim(), balance: balance! });
  }

  openDeactivate(product: Product): void {
    this.deactivatingProduct.set(product);
  }

  closeDeactivate(): void {
    this.deactivatingProduct.set(null);
  }

  confirmDeactivate(): void {
    this.deactivateMutation.mutate();
  }
}
