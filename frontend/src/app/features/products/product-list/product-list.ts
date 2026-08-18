import { Component, OnInit, signal } from '@angular/core';
import { TableModule } from 'primeng/table';
import { Product } from '../../../core/models/product.model';
import { ProductService } from '../../../core/services/product.service';
import { ProductForm } from '../product-form/product-form';

@Component({
  selector: 'app-product-list',
  standalone: true,
  imports: [TableModule, ProductForm],
  templateUrl: './product-list.html',
  styleUrl: './product-list.scss',
})
export class ProductList implements OnInit {
  products = signal<Product[]>([]);
  isLoading = signal(false);

  constructor(private productService: ProductService) {}

  ngOnInit(): void {
    this.loadProducts();
  }

  loadProducts(): void {
    this.isLoading.set(true);
    this.productService.getAll().subscribe({
      next: (products) => {
        this.products.set(products);
        this.isLoading.set(false);
      },
      error: () => {
        this.isLoading.set(false);
      },
    });
  }

  onProductCreated(): void {
    this.loadProducts();
  }
}
