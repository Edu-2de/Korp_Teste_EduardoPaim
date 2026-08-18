import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { AddInvoiceItemDto, Invoice } from '../models/invoice.model';

@Injectable({ providedIn: 'root' })
export class InvoiceService {
  private readonly baseUrl = `${environment.apiUrl}/invoices`;

  constructor(private http: HttpClient) {}

  getAll(): Observable<Invoice[]> {
    return this.http.get<Invoice[]>(this.baseUrl);
  }

  getById(id: string): Observable<Invoice> {
    return this.http.get<Invoice>(`${this.baseUrl}/${id}`);
  }

  create(): Observable<Invoice> {
    return this.http.post<Invoice>(this.baseUrl, null);
  }

  addItem(invoiceId: string, dto: AddInvoiceItemDto): Observable<void> {
    return this.http.post<void>(`${this.baseUrl}/${invoiceId}/items`, dto);
  }

  print(invoiceId: string): Observable<void> {
    return this.http.post<void>(`${this.baseUrl}/${invoiceId}/print`, null);
  }
}
