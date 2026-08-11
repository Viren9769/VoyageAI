import { ComponentFixture, TestBed } from '@angular/core/testing';

import { Aiplanner } from './aiplanner';

describe('Aiplanner', () => {
  let component: Aiplanner;
  let fixture: ComponentFixture<Aiplanner>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [Aiplanner],
    }).compileComponents();

    fixture = TestBed.createComponent(Aiplanner);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
