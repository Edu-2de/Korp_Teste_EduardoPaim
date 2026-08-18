import { CommonModule } from '@angular/common';
import { Component, OnInit, signal } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { ButtonModule } from 'primeng/button';
import { InputNumberModule } from 'primeng/inputnumber';
import { MessageModule } from 'primeng/message';
import { SelectModule } from 'primeng/select';
import { TableModule } from 'primeng/table';
import { TagModule } from 'primeng/tag';
import { Invoice } from '../../../core/models/invoice.model';
import { Product } from '../../../core/models/product.model';
import { InvoiceService } from '../../../core/services/invoice.service';
import { ProductService } from '../../../core/services/product.service';

@Component({
  selector: 'app-invoice-detail',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    ButtonModule,
    TableModule,
    TagModule,
    SelectModule,
    InputNumberModule,
    MessageModule,
  ],
  templateUrl: './invoice-detail.html',
  styleUrl: './invoice-detail.scss',
})
export class InvoiceDetail implements OnInit {
  invoice: Invoice | null = null;

  isLoading = false;
  isAddingItem = false;
  isPrinting = false;
  errorMessage: string | undefined;

  itemForm: FormGroup;

  constructor(
    private route: ActivatedRoute,
    private invoiceService: InvoiceService,
    private productService: ProductService,
    private fb: FormBuilder,
  ) {
    this.itemForm = this.fb.group({
      productId: [null, Validators.required],
      quantity: [1, [Validators.required, Validators.min(1)]],
    });
  }

  ngOnInit(): void {
    this.loadProducts();
    this.loadInvoice();
  }

  get invoiceId(): string {
    return this.route.snapshot.paramMap.get('id')!;
  }

  loadInvoice(): void {
    this.isLoading = true;
    this.invoiceService.getById(this.invoiceId).subscribe({
      next: (invoice) => {
        this.invoice = invoice;
        this.isLoading = false;
      },
      error: () => {
        this.isLoading = false;
        this.errorMessage = 'Não foi possível carregar a nota fiscal.';
      },
    });
  }

  products = signal<Product[]>([]);

  loadProducts(): void {
    this.productService.getAll().subscribe({
      next: (products) => this.products.set(products),
    });
  }

  addItem(): void {
    if (this.itemForm.invalid) return;

    this.isAddingItem = true;
    this.errorMessage = undefined;

    this.invoiceService.addItem(this.invoiceId, this.itemForm.value).subscribe({
      next: () => {
        this.isAddingItem = false;
        this.itemForm.reset({ productId: null, quantity: 1 });
        this.loadInvoice();
      },
      error: (err) => {
        this.isAddingItem = false;
        this.errorMessage = err.error?.message ?? 'Erro ao adicionar item.';
      },
    });
  }

  printInvoice(): void {
    this.isPrinting = true;
    this.errorMessage = undefined;

    this.invoiceService.print(this.invoiceId).subscribe({
      next: () => {
        this.isPrinting = false;
        this.loadInvoice();
      },
      error: (err) => {
        this.isPrinting = false;
        this.errorMessage = err.error?.message ?? 'Erro ao imprimir a nota fiscal.';
      },
    });
  }
}
