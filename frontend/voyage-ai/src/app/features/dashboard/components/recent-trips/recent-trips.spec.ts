import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RecentTrips } from './recent-trips';

describe('RecentTrips', () => {
  let component: RecentTrips;
  let fixture: ComponentFixture<RecentTrips>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [RecentTrips],
    }).compileComponents();

    fixture = TestBed.createComponent(RecentTrips);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
