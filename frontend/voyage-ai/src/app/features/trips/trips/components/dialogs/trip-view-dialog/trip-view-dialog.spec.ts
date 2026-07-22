import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TripViewDialog } from './trip-view-dialog';

describe('TripViewDialog', () => {
  let component: TripViewDialog;
  let fixture: ComponentFixture<TripViewDialog>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TripViewDialog],
    }).compileComponents();

    fixture = TestBed.createComponent(TripViewDialog);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
