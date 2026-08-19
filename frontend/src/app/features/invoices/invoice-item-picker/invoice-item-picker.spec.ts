import { ComponentFixture, TestBed } from '@angular/core/testing';

import { InvoiceItemPicker } from './invoice-item-picker';

describe('InvoiceItemPicker', () => {
  let component: InvoiceItemPicker;
  let fixture: ComponentFixture<InvoiceItemPicker>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [InvoiceItemPicker],
    }).compileComponents();

    fixture = TestBed.createComponent(InvoiceItemPicker);
    component = fixture.componentInstance;
    fixture.componentRef.setInput('products', []);
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
