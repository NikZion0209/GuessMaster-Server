import { ComponentFixture, TestBed } from '@angular/core/testing';

import { WordyWondersComponent } from './wordy-wonders.component';

describe('WordyWondersComponent', () => {
  let component: WordyWondersComponent;
  let fixture: ComponentFixture<WordyWondersComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [WordyWondersComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(WordyWondersComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
