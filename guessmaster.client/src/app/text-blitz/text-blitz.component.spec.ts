import { ComponentFixture, TestBed } from '@angular/core/testing';

import { textBlitzComponent } from './text-blitz.component';

describe('textBlitzComponent', () => {
  let component: textBlitzComponent;
  let fixture: ComponentFixture<textBlitzComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [textBlitzComponent]
    })
      .compileComponents();

    fixture = TestBed.createComponent(textBlitzComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
