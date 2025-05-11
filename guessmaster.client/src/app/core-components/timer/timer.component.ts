import { Component, Input, AfterViewInit } from '@angular/core';
import { TimerService } from '../../services/timer.service';

@Component({
    selector: 'app-timer',
    templateUrl: './timer.component.html',
    styleUrls: ['./timer.component.css'],
    standalone: false
})

export class TimerComponent implements AfterViewInit {
  @Input() time!: number;

  constructor(private timerService: TimerService) {}

  ngAfterViewInit() {
    this.timerService.time$.subscribe(time => {
      this.time = time;
    });
  }
}