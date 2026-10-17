import { ComponentFixture, TestBed } from '@angular/core/testing';

import { QuarantineList } from './quarantine-list';

describe('QuarantineList', () => {
  let component: QuarantineList;
  let fixture: ComponentFixture<QuarantineList>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [QuarantineList],
    }).compileComponents();

    fixture = TestBed.createComponent(QuarantineList);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
