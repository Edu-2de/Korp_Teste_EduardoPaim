import { Injectable } from '@angular/core';
import { apiClient } from '../http/api-client';
import { CreateProductDto, Product } from '../models/product.model';

@Injectable({ providedIn: 'root' })
export class ProductService {
  private readonly baseUrl = '/products';

  getAll(): Promise<Product[]> {
    return apiClient.get<Product[]>(this.baseUrl).then((res) => res.data);
  }

  create(dto: CreateProductDto): Promise<Product> {
    return apiClient.post<Product>(this.baseUrl, dto).then((res) => res.data);
  }

  updateDescription(id: string, description: string): Promise<void> {
    return apiClient
      .patch<void>(`${this.baseUrl}/${id}/description`, { description })
      .then(() => undefined);
  }

  updateBalance(id: string, balance: number): Promise<void> {
    return apiClient
      .patch<void>(`${this.baseUrl}/${id}/balance`, { balance })
      .then(() => undefined);
  }

  deactivate(id: string): Promise<void> {
    return apiClient.delete<void>(`${this.baseUrl}/${id}`).then(() => undefined);
  }
}
