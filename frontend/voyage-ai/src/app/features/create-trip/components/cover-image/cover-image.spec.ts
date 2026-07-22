import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CoverImage } from './cover-image';

describe('CoverImage', () => {
  let component: CoverImage;
  let fixture: ComponentFixture<CoverImage>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CoverImage],
    }).compileComponents();

    fixture = TestBed.createComponent(CoverImage);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
