export interface Product {
  id: string;
  code: string;
  description: string;
  balance: number;
  isActive: boolean;
}

export interface CreateProductDto {
  code: string;
  description: string;
  balance: number;
}
