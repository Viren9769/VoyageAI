import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TravelerPrfile } from './traveler-prfile';

describe('TravelerPrfile', () => {
  let component: TravelerPrfile;
  let fixture: ComponentFixture<TravelerPrfile>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TravelerPrfile],
    }).compileComponents();

    fixture = TestBed.createComponent(TravelerPrfile);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
