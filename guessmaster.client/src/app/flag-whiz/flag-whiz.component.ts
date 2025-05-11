import { Component } from '@angular/core';
import { TimerService } from '../services/timer.service';
import { JsonRetrievalService } from '../services/json-retrieval.service';
import { ScoreService } from '../services/score.service';
import { UtilityService } from '../services/utility.service';
import { AudioService } from '../services/audio.service';

@Component({
    selector: 'app-flag-whiz',
    templateUrl: './flag-whiz.component.html',
    styleUrl: './flag-whiz.component.css',
    standalone: false
})

export class FlagWhizComponent {
  flagsJsonUrl = 'assets/json/flags.json';

  time = 20;
  score = 0;
  gameState = false;

  words: { text: string }[] = [];
  flags: { url: string, word: string }[] = [];

  selectedWord = '';
  currentFlag: { url: string, word: string } | null = null;
  currentIndex = 0;

  constructor(
    private jsonRetrievalService: JsonRetrievalService,
    private timerService: TimerService,
    private scoreService: ScoreService,
    private utilityService: UtilityService,
    private audioService: AudioService
  ) { }

  ngOnInit() {
    this.loadFlags();
    this.timerService.timeUp.subscribe(() => this.onTimeUp());
  }

  ngAfterViewInit() {
    this.gameState = true;
    this.timerService.startTimerDecrementing(this.time);
  }

  ngOnDestroy() {
    if (this.gameState) {
      this.timerService.stopTimer();
    }
    this.scoreService.resetScore();
  }

  onTimeUp() {
    this.gameState = false;
    const totalScore = this.scoreService.retrieveScore();
    alert('Time is up! You scored ' + totalScore + ' points!');
  }

  private loadFlags() {
    this.jsonRetrievalService.getObjects(this.flagsJsonUrl).subscribe(flags => {
      flags = this.utilityService.shuffleArray(flags);

      this.flags = flags;
      this.currentFlag = this.flags[this.currentIndex];

      const randSelection = [...(flags).slice(1, 4).map(flag => flag.word), this.currentFlag.word];

      this.words = this.utilityService.shuffleArray(randSelection.map(word => ({ text: word })));
    });
  }

  onWordSelected(word: { text: string }) {
    this.selectedWord = word.text;
    this.checkAnswer();
  }

  private checkAnswer() {
    if (this.currentFlag && this.selectedWord === this.currentFlag.word) {
      this.audioService.playCorrectSound();
      this.scoreService.addScore(10);
      this.nextFlag();
    } else {
      this.audioService.playIncorrectSound();
    }
  }

  private nextFlag() {
    this.currentIndex++;
    this.currentFlag = this.flags[this.currentIndex];
    this.selectedWord = '';

    const uniqueWords = new Set<string>();

    uniqueWords.add(this.currentFlag.word);

    const shuffledFlags = this.utilityService.shuffleArray(this.flags);
    for (const flag of shuffledFlags) {
      if (uniqueWords.size >= 4) break;
      if (flag.word !== this.currentFlag.word) {
        uniqueWords.add(flag.word);
      }
    }

    const randSelection = Array.from(uniqueWords);
    this.words = this.utilityService.shuffleArray(randSelection.map(word => ({ text: word })));
  }
}
