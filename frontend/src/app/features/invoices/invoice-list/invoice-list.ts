import { DatePipe } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { ButtonModule } from 'primeng/button';
import { TableModule } from 'primeng/table';
import { TagModule } from 'primeng/tag';
import { Invoice } from '../../../core/models/invoice.model';
import { InvoiceService } from '../../../core/services/invoice.service';

@Component({
  selector: 'app-invoice-list',
  standalone: true,
  imports: [TableModule, ButtonModule, TagModule, DatePipe],
  templateUrl: './invoice-list.html',
  styleUrl: './invoice-list.scss',
})
export class InvoiceList implements OnInit {
  invoices: Invoice[] = [];
  isLoading = false;
  isCreating = false;

  constructor(
    private invoiceService: InvoiceService,
    private router: Router,
  ) {}

  ngOnInit(): void {
    this.loadInvoices();
  }

  loadInvoices(): void {
    this.isLoading = true;
    this.invoiceService.getAll().subscribe({
      next: (invoices) => {
        this.invoices = invoices;
        this.isLoading = false;
      },
      error: () => {
        this.isLoading = false;
      },
    });
  }

  createInvoice(): void {
    this.isCreating = true;
    this.invoiceService.create().subscribe({
      next: (invoice) => {
        this.isCreating = false;
        this.router.navigate(['/invoices', invoice.id]);
      },
      error: () => {
        this.isCreating = false;
      },
    });
  }

  openInvoice(invoice: Invoice): void {
    this.router.navigate(['/invoices', invoice.id]);
  }
}
