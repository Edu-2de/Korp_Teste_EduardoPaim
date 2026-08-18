export interface Product {
  id: string;
  code: string;
  description: string;
  balance: number;
}

export interface CreateProductDto {
  code?: string;
  description: string;
  balance: number;
}
