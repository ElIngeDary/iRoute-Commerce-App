import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ProcessCommerce } from './process-commerce';

describe('ProcessCommerce', () => {
  let component: ProcessCommerce;
  let fixture: ComponentFixture<ProcessCommerce>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ProcessCommerce],
    }).compileComponents();

    fixture = TestBed.createComponent(ProcessCommerce);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
