import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TravelMap } from './travel-map';

describe('TravelMap', () => {
  let component: TravelMap;
  let fixture: ComponentFixture<TravelMap>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TravelMap],
    }).compileComponents();

    fixture = TestBed.createComponent(TravelMap);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
