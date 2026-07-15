import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AiPlannerCard } from './ai-planner-card';

describe('AiPlannerCard', () => {
  let component: AiPlannerCard;
  let fixture: ComponentFixture<AiPlannerCard>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AiPlannerCard],
    }).compileComponents();

    fixture = TestBed.createComponent(AiPlannerCard);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
