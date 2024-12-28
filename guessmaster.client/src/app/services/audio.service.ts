import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class AudioService {
  private audioInstances: HTMLAudioElement[] = [];
  private correctSound = new Audio('assets/sounds/correctGuess.mp3');
  private incorrectSound = new Audio('assets/sounds/incorrectGuess.mp3');

  addAudioInstance(audio: HTMLAudioElement) {
    this.audioInstances.push(audio);
  }

  playCorrectSound() { 
    this.correctSound.play();
  }

  playIncorrectSound() {
    this.incorrectSound.play();
  }

  stopAllAudio() {
    this.audioInstances.forEach(audio => {
      audio.pause();
      audio.currentTime = 0;
    });
  }
}
