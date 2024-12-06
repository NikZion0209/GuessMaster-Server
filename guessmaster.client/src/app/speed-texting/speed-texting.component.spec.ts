import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SpeedTextingComponent } from './speed-texting.component';

describe('SpeedTextingComponent', () => {
  let component: SpeedTextingComponent;
  let fixture: ComponentFixture<SpeedTextingComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [SpeedTextingComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(SpeedTextingComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
