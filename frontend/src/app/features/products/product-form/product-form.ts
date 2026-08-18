import { Component, output } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ButtonModule } from 'primeng/button';
import { InputNumberModule } from 'primeng/inputnumber';
import { InputTextModule } from 'primeng/inputtext';
import { ProductService } from '../../../core/services/product.service';

@Component({
  selector: 'app-product-form',
  standalone: true,
  imports: [ReactiveFormsModule, ButtonModule, InputTextModule, InputNumberModule],
  templateUrl: './product-form.html',
  styleUrl: './product-form.scss',
})
export class ProductForm {
  productCreated = output<void>();
  isSubmitting = false;
  form: FormGroup;

  constructor(
    private fb: FormBuilder,
    private productService: ProductService,
  ) {
    this.form = this.fb.group({
      code: [''],
      description: ['', Validators.required],
      balance: [0, [Validators.required, Validators.min(0)]],
    });
  }

  onSubmit(): void {
    if (this.form.invalid) return;

    this.isSubmitting = true;

    this.productService.create(this.form.value as any).subscribe({
      next: () => {
        this.isSubmitting = false;
        this.form.reset({ code: '', description: '', balance: 0 });
        this.productCreated.emit();
      },
      error: () => {
        this.isSubmitting = false;
      },
    });
  }
}
