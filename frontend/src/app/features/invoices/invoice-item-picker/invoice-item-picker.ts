import { Component, computed, effect, inject, input, output } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ButtonModule } from 'primeng/button';
import { InputNumberModule } from 'primeng/inputnumber';
import { SelectModule } from 'primeng/select';
import { Product } from '../../../core/models/product.model';

export interface InvoiceItemPicked {
  productId: string;
  quantity: number;
}

/**
 * Formulário de "produto + quantidade" reutilizado tanto ao criar uma nota
 * (rascunho local) quanto ao editar uma já existente (mutation direta) —
 * as duas telas compartilham a mesma regra de saldo máximo.
 */
@Component({
  selector: 'app-invoice-item-picker',
  standalone: true,
  imports: [ReactiveFormsModule, SelectModule, InputNumberModule, ButtonModule],
  templateUrl: './invoice-item-picker.html',
})
export class InvoiceItemPicker {
  products = input.required<Product[]>();
  submitLabel = input('Adicionar');
  pending = input(false);

  picked = output<InvoiceItemPicked>();

  private fb = inject(FormBuilder);

  form = this.fb.group({
    productId: [null as string | null, Validators.required],
    quantity: [1, [Validators.required, Validators.min(1)]],
  });

  private selectedProductId = toSignal(this.form.controls.productId.valueChanges, {
    initialValue: null as string | null,
  });

  selectedProduct = computed(() => {
    const id = this.selectedProductId();
    return this.products().find((product) => product.id === id) ?? null;
  });

  constructor() {
    effect(() => {
      const max = this.selectedProduct()?.balance ?? null;
      const quantityControl = this.form.controls.quantity;
      quantityControl.setValidators(
        max != null
          ? [Validators.required, Validators.min(1), Validators.max(max)]
          : [Validators.required, Validators.min(1)],
      );
      quantityControl.updateValueAndValidity({ emitEvent: false });
    });
  }

  submit(): void {
    if (this.form.invalid) return;
    const { productId, quantity } = this.form.value;
    this.picked.emit({ productId: productId!, quantity: quantity! });
  }

  /** Chamado pelo componente pai depois que o item foi de fato aceito (mutation ok, ou adicionado ao rascunho). */
  reset(): void {
    this.form.reset({ productId: null, quantity: 1 });
  }
}
