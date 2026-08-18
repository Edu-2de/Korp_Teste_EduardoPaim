import { ComponentFixture, TestBed } from '@angular/core/testing';

import { InvoiceDetail } from './invoice-detail';

describe('InvoiceDetail', () => {
  let component: InvoiceDetail;
  let fixture: ComponentFixture<InvoiceDetail>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [InvoiceDetail],
    }).compileComponents();

    fixture = TestBed.createComponent(InvoiceDetail);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
