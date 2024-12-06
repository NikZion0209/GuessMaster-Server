import { ComponentFixture, TestBed } from '@angular/core/testing';

import { GuessTheCountryComponent } from './guess-the-country.component';

describe('GuessTheCountryComponent', () => {
  let component: GuessTheCountryComponent;
  let fixture: ComponentFixture<GuessTheCountryComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [GuessTheCountryComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(GuessTheCountryComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
