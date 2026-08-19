import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { QueryClient, provideTanStackQuery } from '@tanstack/angular-query-experimental';
import { MessageService } from 'primeng/api';

import { InvoiceList } from './invoice-list';

describe('InvoiceList', () => {
  let component: InvoiceList;
  let fixture: ComponentFixture<InvoiceList>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [InvoiceList],
      providers: [provideRouter([]), provideTanStackQuery(new QueryClient()), MessageService],
    }).compileComponents();

    fixture = TestBed.createComponent(InvoiceList);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
