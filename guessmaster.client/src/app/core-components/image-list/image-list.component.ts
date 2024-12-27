import { Component, Input, Output, EventEmitter } from '@angular/core';

@Component({
  selector: 'app-image-list',
  templateUrl: './image-list.component.html',
  styleUrls: ['./image-list.component.css']
})

export class ImageListComponent {
  @Input() images: { url: string, word: string, disabled: boolean }[] = [];
  @Output() imageSelected = new EventEmitter<{ image: { url: string, word: string, disabled: boolean }, mouse: MouseEvent }>();

  selectImage(image: { url: string, word: string, disabled: boolean }, mouse: MouseEvent) {
    this.imageSelected.emit({ image, mouse });
  }
}
