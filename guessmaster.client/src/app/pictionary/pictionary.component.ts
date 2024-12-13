import { Component, ElementRef, ViewChild } from '@angular/core';
import { TimerService } from '../services/timer.service';
import { AnimalService } from '../services/animal-service.service';
import { ScoreService } from '../services/score.service';

@Component({
  selector: 'app-pictionary',
  templateUrl: './pictionary.component.html',
  styleUrls: ['./pictionary.component.css']
})

export class PictionaryComponent {
  @ViewChild('matchCanvas', { static: true }) canvas!: ElementRef<HTMLCanvasElement>;

  words: { text: string, disabled: boolean }[] = [];
  images: { url: string, word: string, disabled: boolean }[] = [];

  time = 5;
  score = 0;

  gameState = false;

  selectedWord = '';
  selectedImage: { url: string, word: string, disabled: boolean } | null = null;
  selectedWordPosition: { x: number, y: number } | null = null;
  selectedImagePosition: { x: number, y: number } | null = null;

  correctSound = new Audio('assets/sounds/correctGuess.mp3');
  incorrectSound = new Audio('assets/sounds/incorrectGuess.mp3');

  constructor(
    private animalService: AnimalService,
    private timerService: TimerService,
    private scoreService: ScoreService
  ) {}

  ngOnInit() {
    this.loadAnimals();
    this.timerService.timeUp.subscribe(() => this.onTimeUp());
  }

  ngAfterViewInit() {
    this.gameState = true;
    this.timerService.startTimer(this.time);
  }

  ngOnDestroy() {
    if (this.gameState){
      this.timerService.stopTimer();
    }
    this.scoreService.resetScore();
  }

  loadAnimals() {
    this.animalService.getAnimals().subscribe(animals => {
      if (animals.length === 0) {
        // Fallback data if the HTTP request fails
       
      }
  
      animals = this.shuffleArray(animals);
      const selectedAnimals = animals.slice(0, 4);
  
      this.words = this.shuffleArray(selectedAnimals.map(animal => ({ text: animal.word, disabled: false })));
      this.images = this.shuffleArray(selectedAnimals.map(animal => ({ ...animal, disabled: false })));
    });
  }
  
  shuffleArray(array: any[]): any[] {
    for (let i = array.length - 1; i > 0; i--) {
      const j = Math.floor(Math.random() * (i + 1));
      [array[i], array[j]] = [array[j], array[i]];
    }
    return array;
  }

  onTimeUp() {
    this.gameState = false;
    const totalScore = this.scoreService.retrieveScore();
    alert('Time is up! You scored ' + totalScore + ' points!');
  }

  drawLine(start: { x: number, y: number }, end: { x: number, y: number }) {
    const canvas = this.canvas.nativeElement;
    const context = canvas.getContext('2d');
    if (context) {
      context.beginPath();
      context.moveTo(start.x, start.y);
      context.lineTo(end.x, end.y);
      context.stroke();
    }
  }
  
  getMousePosition(event: MouseEvent, type: string) {
    const canvasRect = this.canvas.nativeElement.getBoundingClientRect();
    const scaleX = this.canvas.nativeElement.width / canvasRect.width;
    const scaleY = this.canvas.nativeElement.height / canvasRect.height;
  
    const clickPosition = {
      x: (event.clientX - canvasRect.left) * scaleX,
      y: (event.clientY - canvasRect.top) * scaleY
    };
  
    if (type === 'word') {
      this.selectedWordPosition = clickPosition;
    } else {
      this.selectedImagePosition = clickPosition;
    }
  }

  onWordSelected(word: { text: string, disabled: boolean }, mouse: MouseEvent) {
    if (word.disabled) return;
    this.selectedWord = word.text;
    this.getMousePosition(mouse, 'word');
    if (this.selectedImage) {
      this.checkGuess(word.text, this.selectedImage);
    }
  }

  onImageSelected(image: { url: string, word: string, disabled: boolean }, mouse: MouseEvent) {
    if (image.disabled) return;
    this.selectedImage = image;
    this.getMousePosition(mouse, 'image');
    if (this.selectedWord) {
      this.checkGuess(this.selectedWord, image);
    }
  }

  checkGuess(word: string, image: { url: string, word: string, disabled: boolean }) {
    //correct guess
    if (word === image.word && this.selectedWordPosition && this.selectedImagePosition) {
      this.drawLine(this.selectedWordPosition, this.selectedImagePosition);
      this.words.find(w => w.text === word)!.disabled = true;
      this.images.find(img => img.word === word)!.disabled = true;
      this.scoreService.addScore(10);
      this.correctSound.play();
    } 
    //incorrect guess
    else {
      this.incorrectSound.play();
    }

    this.selectedImage = null;
    this.selectedWord = '';
    this.selectedWordPosition = null;
    this.selectedImagePosition = null;

    if (this.words.every(w => w.disabled) && this.images.every(img => img.disabled)) {
      this.scoreService.addScore(50);
      this.resetGame();
    }
  }

  resetGame() {
    this.loadAnimals();
    const canvas = this.canvas.nativeElement;
    const context = canvas.getContext('2d');
    if (context) {
      context.clearRect(0, 0, canvas.width, canvas.height);
    }
  }
}