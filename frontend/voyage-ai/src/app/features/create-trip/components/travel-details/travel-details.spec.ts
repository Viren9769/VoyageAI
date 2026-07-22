import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TravelDetails } from './travel-details';

describe('TravelDetails', () => {
  let component: TravelDetails;
  let fixture: ComponentFixture<TravelDetails>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TravelDetails],
    }).compileComponents();

    fixture = TestBed.createComponent(TravelDetails);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
