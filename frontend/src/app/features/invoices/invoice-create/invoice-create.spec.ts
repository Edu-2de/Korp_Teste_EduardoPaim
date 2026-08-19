import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { QueryClient, provideTanStackQuery } from '@tanstack/angular-query-experimental';
import { MessageService } from 'primeng/api';

import { InvoiceCreate } from './invoice-create';

describe('InvoiceCreate', () => {
  let component: InvoiceCreate;
  let fixture: ComponentFixture<InvoiceCreate>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [InvoiceCreate],
      providers: [provideRouter([]), provideTanStackQuery(new QueryClient()), MessageService],
    }).compileComponents();

    fixture = TestBed.createComponent(InvoiceCreate);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
