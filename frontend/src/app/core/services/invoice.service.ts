import { Injectable } from '@angular/core';
import { apiClient } from '../http/api-client';
import { AddInvoiceItemDto, Invoice } from '../models/invoice.model';

@Injectable({ providedIn: 'root' })
export class InvoiceService {
  private readonly baseUrl = '/invoices';

  getAll(): Promise<Invoice[]> {
    return apiClient.get<Invoice[]>(this.baseUrl).then((res) => res.data);
  }

  getById(id: string): Promise<Invoice> {
    return apiClient.get<Invoice>(`${this.baseUrl}/${id}`).then((res) => res.data);
  }

  create(): Promise<Invoice> {
    return apiClient.post<Invoice>(this.baseUrl, null).then((res) => res.data);
  }

  addItem(invoiceId: string, dto: AddInvoiceItemDto): Promise<void> {
    return apiClient.post<void>(`${this.baseUrl}/${invoiceId}/items`, dto).then(() => undefined);
  }

  removeItem(invoiceId: string, itemId: string): Promise<void> {
    return apiClient
      .delete<void>(`${this.baseUrl}/${invoiceId}/items/${itemId}`)
      .then(() => undefined);
  }

  print(invoiceId: string): Promise<void> {
    return apiClient.post<void>(`${this.baseUrl}/${invoiceId}/print`, null).then(() => undefined);
  }

  delete(invoiceId: string): Promise<void> {
    return apiClient.delete<void>(`${this.baseUrl}/${invoiceId}`).then(() => undefined);
  }
}
