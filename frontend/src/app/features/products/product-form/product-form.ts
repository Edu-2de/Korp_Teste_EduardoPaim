import { Component, inject } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { QueryClient, injectMutation } from '@tanstack/angular-query-experimental';
import { MessageService } from 'primeng/api';
import { ButtonModule } from 'primeng/button';
import { InputNumberModule } from 'primeng/inputnumber';
import { InputTextModule } from 'primeng/inputtext';
import { apiErrorMessage } from '../../../core/http/api-client';
import { ProductService } from '../../../core/services/product.service';

@Component({
  selector: 'app-product-form',
  standalone: true,
  imports: [ReactiveFormsModule, ButtonModule, InputTextModule, InputNumberModule],
  templateUrl: './product-form.html',
  styleUrl: './product-form.scss',
})
export class ProductForm {
  form: FormGroup;

  private queryClient = inject(QueryClient);

  createMutation = injectMutation(() => ({
    mutationFn: () => this.productService.create(this.form.value),
    onSuccess: () => {
      this.queryClient.invalidateQueries({ queryKey: ['products'] });
      this.form.reset({ code: '', description: '', balance: 0 });
      this.messageService.add({
        severity: 'success',
        summary: 'Produto criado',
        detail: 'O produto foi cadastrado com sucesso.',
      });
    },
    onError: (err) => {
      this.messageService.add({
        severity: 'error',
        summary: 'Erro ao criar produto',
        detail: apiErrorMessage(err, 'Não foi possível cadastrar o produto.'),
      });
    },
  }));

  constructor(
    private fb: FormBuilder,
    private productService: ProductService,
    private messageService: MessageService,
  ) {
    this.form = this.fb.group({
      code: ['', Validators.required],
      description: ['', Validators.required],
      balance: [0, [Validators.required, Validators.min(0)]],
    });
  }

  onSubmit(): void {
    if (this.form.invalid) return;
    this.createMutation.mutate();
  }
}
