import { ComponentFixture, TestBed } from '@angular/core/testing';

import { UploadCsv } from './upload-csv';

describe('UploadCsv', () => {
  let component: UploadCsv;
  let fixture: ComponentFixture<UploadCsv>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [UploadCsv],
    }).compileComponents();

    fixture = TestBed.createComponent(UploadCsv);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
