import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TripFilters } from './trip-filters';

describe('TripFilters', () => {
  let component: TripFilters;
  let fixture: ComponentFixture<TripFilters>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TripFilters],
    }).compileComponents();

    fixture = TestBed.createComponent(TripFilters);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
