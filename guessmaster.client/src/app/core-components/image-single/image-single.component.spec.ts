import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ImageSingleComponent } from './image-single.component';

describe('ImageSingleComponent', () => {
  let component: ImageSingleComponent;
  let fixture: ComponentFixture<ImageSingleComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ImageSingleComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(ImageSingleComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
