import { ComponentFixture, TestBed } from '@angular/core/testing';

import { Travelers } from './travelers';

describe('Travelers', () => {
  let component: Travelers;
  let fixture: ComponentFixture<Travelers>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [Travelers],
    }).compileComponents();

    fixture = TestBed.createComponent(Travelers);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
