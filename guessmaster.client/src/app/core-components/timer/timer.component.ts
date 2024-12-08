import { Component, EventEmitter, Input, Output } from '@angular/core';

@Component({
  selector: 'app-timer',
  templateUrl: './timer.component.html',
  styleUrls: ['./timer.component.css']
})
export class TimerComponent {
  @Input() time!: number;
  intervalId: any;

  @Output() timeUp = new EventEmitter<void>();

  startTimer() {
    this.intervalId = setInterval(() => {
      if (this.time > 0) {
        this.time--;
      } else {
        this.stopTimer();
        this.timeUp.emit();
      }
    }, 1000);
  }

  stopTimer() {
    clearInterval(this.intervalId);
  }
}
