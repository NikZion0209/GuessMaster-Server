import { Component, ViewChild } from '@angular/core';
import { TimerComponent } from '../core-components/timer/timer.component';

@Component({
  selector: 'app-flag-whiz',
  templateUrl: './flag-whiz.component.html',
  styleUrl: './flag-whiz.component.css'
})
export class FlagWhizComponent {
  @ViewChild(TimerComponent) timerComponent!: TimerComponent;

  time = 10;
  score = 0;

  // After initialision
  ngAfterViewInit() {
    //this.timerComponent.startTimer();
  }
}
