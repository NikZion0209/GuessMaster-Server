import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class AudioService {
  private audioInstances: HTMLAudioElement[] = [];

  addAudioInstance(audio: HTMLAudioElement) {
    this.audioInstances.push(audio);
  }

  stopAllAudio() {
    this.audioInstances.forEach(audio => {
      audio.pause();
      audio.currentTime = 0;
    });
  }
}
