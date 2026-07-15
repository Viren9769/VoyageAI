import { ComponentFixture, TestBed } from '@angular/core/testing';

import { WelcomeCard } from './welcome-card';

describe('WelcomeCard', () => {
  let component: WelcomeCard;
  let fixture: ComponentFixture<WelcomeCard>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [WelcomeCard],
    }).compileComponents();

    fixture = TestBed.createComponent(WelcomeCard);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
