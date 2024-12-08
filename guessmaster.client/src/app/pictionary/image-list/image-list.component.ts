import { Component, Input, Output, EventEmitter, OnInit } from '@angular/core';

@Component({
  selector: 'app-image-list',
  templateUrl: './image-list.component.html',
  styleUrls: ['./image-list.component.css']
})
export class ImageListComponent implements OnInit {
  @Input() images: { url: string, word: string, disabled: boolean }[] = [];
  @Output() imageSelected = new EventEmitter<{ image: { url: string, word: string, disabled: boolean }, mouse: MouseEvent }>();

  ngOnInit() {
    this.shuffleImages();
  }

  shuffleImages() {
    for (let i = this.images.length - 1; i > 0; i--) {
      const j = Math.floor(Math.random() * (i + 1));
      [this.images[i], this.images[j]] = [this.images[j], this.images[i]];
    }
  }

  selectImage(image: { url: string, word: string, disabled: boolean }, mouse: MouseEvent) {
    this.imageSelected.emit({ image, mouse });
  }
}
