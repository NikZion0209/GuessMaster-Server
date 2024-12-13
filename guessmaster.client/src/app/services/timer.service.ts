import { Injectable, EventEmitter, Output } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { AudioService } from './audio.service';

@Injectable({
  providedIn: 'root'
})

export class TimerService {
  private timeSubject = new BehaviorSubject<number>(0);
  time$ = this.timeSubject.asObservable();

  intervalId: any;
  sixtySecondCountdown = new Audio('assets/sounds/countdownSixty.mp3');

  @Output() timeUp = new EventEmitter<void>();

  constructor(private audioService: AudioService) {}

  startTimer(time: number) {
    this.audioService.addAudioInstance(this.sixtySecondCountdown);
    this.sixtySecondCountdown.play();
    this.timeSubject.next(time);
    this.intervalId = setInterval(() => {
      if (time > 0) {
        time--;
        this.timeSubject.next(time);
      } else {
        this.stopTimer();
        this.onTimeUp();
      }
    }, 1000);
  }

  stopTimer() {
    clearInterval(this.intervalId);
    this.audioService.stopAllAudio();
  }

  private onTimeUp() {
    this.timeUp.emit();
  }
}