import { Component, ViewChild } from '@angular/core';
import { TimerComponent } from '../core-components/timer/timer.component';

@Component({
  selector: 'app-guess-the-country',
  templateUrl: './guess-the-country.component.html',
  styleUrl: './guess-the-country.component.css'
})
export class GuessTheCountryComponent {
  @ViewChild(TimerComponent) timerComponent!: TimerComponent;

  time = 10;
  score = 0;

  // After initialision
  ngAfterViewInit() {
    //this.timerComponent.startTimer();
  }
}
