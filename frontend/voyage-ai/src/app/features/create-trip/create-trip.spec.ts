import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CreateTrip } from './create-trip';

describe('CreateTrip', () => {
  let component: CreateTrip;
  let fixture: ComponentFixture<CreateTrip>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CreateTrip],
    }).compileComponents();

    fixture = TestBed.createComponent(CreateTrip);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
