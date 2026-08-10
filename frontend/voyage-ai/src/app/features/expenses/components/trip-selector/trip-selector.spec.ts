import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TripSelector } from './trip-selector';

describe('TripSelector', () => {
  let component: TripSelector;
  let fixture: ComponentFixture<TripSelector>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TripSelector],
    }).compileComponents();

    fixture = TestBed.createComponent(TripSelector);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
