import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CathphraseComponent } from './cathphrase.component';

describe('CathphraseComponent', () => {
  let component: CathphraseComponent;
  let fixture: ComponentFixture<CathphraseComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [CathphraseComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(CathphraseComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
