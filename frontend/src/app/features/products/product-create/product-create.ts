import { Component, inject } from '@angular/core';
import { Router } from '@angular/router';
import { ProductForm } from '../product-form/product-form';

@Component({
  selector: 'app-product-create',
  standalone: true,
  imports: [ProductForm],
  templateUrl: './product-create.html',
  styleUrl: './product-create.scss',
})
export class ProductCreate {
  private router = inject(Router);

  goBack(): void {
    this.router.navigate(['/products']);
  }
}
