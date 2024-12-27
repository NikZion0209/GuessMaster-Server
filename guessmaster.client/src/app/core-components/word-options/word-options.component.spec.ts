import { ComponentFixture, TestBed } from '@angular/core/testing';

import { WordOptionsComponent } from './word-options.component';

describe('WordOptionsComponent', () => {
  let component: WordOptionsComponent;
  let fixture: ComponentFixture<WordOptionsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [WordOptionsComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(WordOptionsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
