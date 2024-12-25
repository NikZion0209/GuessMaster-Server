import { ComponentFixture, TestBed } from '@angular/core/testing';

import { FlagWhizComponent } from './flag-whiz.component';

describe('FlagWhizComponent', () => {
  let component: FlagWhizComponent;
  let fixture: ComponentFixture<FlagWhizComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [FlagWhizComponent]
    })
      .compileComponents();

    fixture = TestBed.createComponent(FlagWhizComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
