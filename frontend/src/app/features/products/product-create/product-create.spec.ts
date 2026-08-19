import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { QueryClient, provideTanStackQuery } from '@tanstack/angular-query-experimental';
import { MessageService } from 'primeng/api';

import { ProductCreate } from './product-create';

describe('ProductCreate', () => {
  let component: ProductCreate;
  let fixture: ComponentFixture<ProductCreate>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ProductCreate],
      providers: [provideRouter([]), provideTanStackQuery(new QueryClient()), MessageService],
    }).compileComponents();

    fixture = TestBed.createComponent(ProductCreate);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
