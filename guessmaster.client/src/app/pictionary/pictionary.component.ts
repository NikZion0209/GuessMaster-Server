import { Component, AfterViewInit, ElementRef, ViewChild } from '@angular/core';

@Component({
  selector: 'app-pictionary',
  templateUrl: './pictionary.component.html',
  styleUrls: ['./pictionary.component.css']
})

export class PictionaryComponent {
  @ViewChild('matchCanvas', { static: true })
  canvas!: ElementRef<HTMLCanvasElement>;

  words = [
    { text: 'Cat', disabled: false },
    { text: 'Dog', disabled: false },
    { text: 'Bird', disabled: false },
    { text: 'Fish', disabled: false }
  ];

  images = [
    { url: '../../assets/animals/cat.jpeg', word: 'Cat', disabled: false },
    { url: '../../assets/animals/dog.jpeg', word: 'Dog', disabled: false },
    { url: '../../assets/animals/bird.jpeg', word: 'Bird', disabled: false },
    { url: '../../assets/animals/fish.jpeg', word: 'Fish', disabled: false }
  ];

  selectedWord = '';
  selectedImage: { url: string, word: string, disabled: boolean } | null = null;
  selectedWordPosition: { x: number, y: number } | null = null;
  selectedImagePosition: { x: number, y: number } | null = null;

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
    if (word === image.word && this.selectedWordPosition && this.selectedImagePosition) {
      this.drawLine(this.selectedWordPosition, this.selectedImagePosition);
      this.words.find(w => w.text === word)!.disabled = true;
      this.images.find(img => img.word === word)!.disabled = true;
    }
    this.selectedImage = null;
    this.selectedWord = '';
    this.selectedWordPosition = null;
    this.selectedImagePosition = null;
  }
}