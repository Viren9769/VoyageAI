import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CreateTripButton } from './create-trip-button';

describe('CreateTripButton', () => {
  let component: CreateTripButton;
  let fixture: ComponentFixture<CreateTripButton>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CreateTripButton],
    }).compileComponents();

    fixture = TestBed.createComponent(CreateTripButton);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
