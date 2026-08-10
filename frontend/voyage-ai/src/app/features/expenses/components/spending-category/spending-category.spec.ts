import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SpendingCategory } from './spending-category';

describe('SpendingCategory', () => {
  let component: SpendingCategory;
  let fixture: ComponentFixture<SpendingCategory>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [SpendingCategory],
    }).compileComponents();

    fixture = TestBed.createComponent(SpendingCategory);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
