export interface InvoiceItem {
  id: string;
  productId: string;
  quantity: number;
}

export interface Invoice {
  id: string;
  number: number;
  status: 'Open' | 'Closed';
  createdAt: string;
  items: InvoiceItem[];
}

export interface AddInvoiceItemDto {
  productId: string;
  quantity: number;
}
