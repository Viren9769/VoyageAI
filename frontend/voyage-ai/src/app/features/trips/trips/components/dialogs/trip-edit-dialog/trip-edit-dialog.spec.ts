import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TripEditDialog } from './trip-edit-dialog';

describe('TripEditDialog', () => {
  let component: TripEditDialog;
  let fixture: ComponentFixture<TripEditDialog>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TripEditDialog],
    }).compileComponents();

    fixture = TestBed.createComponent(TripEditDialog);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
