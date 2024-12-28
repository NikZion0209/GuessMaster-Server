import { Component } from '@angular/core';
import { TimerService } from '../services/timer.service';
import { JsonRetrievalService } from '../services/json-retrieval.service';
import { ScoreService } from '../services/score.service';
import { UtilityService } from '../services/utility.service';

@Component({
  selector: 'app-flag-whiz',
  templateUrl: './flag-whiz.component.html',
  styleUrl: './flag-whiz.component.css'
})

export class FlagWhizComponent {
  flagsJsonUrl = 'assets/json/flags.json';

  time = 60;
  score = 0;
  gameState = false;

  words: { text: string }[] = [];
  flags: { url: string, word: string }[] = [];

  selectedWord = '';
  currentFlag: { url: string, word: string } | null = null;
  currentIndex = 0;

  correctSound = new Audio('assets/sounds/correctGuess.mp3');
  incorrectSound = new Audio('assets/sounds/incorrectGuess.mp3');

  constructor(
    private jsonRetrievalService: JsonRetrievalService,
    private timerService: TimerService,
    private scoreService: ScoreService,
    private utilityService: UtilityService
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
      this.correctSound.play();
      this.scoreService.addScore(10);
      this.nextFlag();
    } else {
      this.incorrectSound.play();
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
