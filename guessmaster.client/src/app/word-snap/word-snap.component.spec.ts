import { ComponentFixture, TestBed } from '@angular/core/testing';

import { WordSnapComponent } from './word-snap.component';

describe('WordSnapComponent', () => {
  let component: WordSnapComponent;
  let fixture: ComponentFixture<WordSnapComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [WordSnapComponent]
    })
      .compileComponents();

    fixture = TestBed.createComponent(WordSnapComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
