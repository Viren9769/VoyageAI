import { ComponentFixture, TestBed } from '@angular/core/testing';

import { UpcomingTrip } from './upcoming-trip';

describe('UpcomingTrip', () => {
  let component: UpcomingTrip;
  let fixture: ComponentFixture<UpcomingTrip>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [UpcomingTrip],
    }).compileComponents();

    fixture = TestBed.createComponent(UpcomingTrip);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
