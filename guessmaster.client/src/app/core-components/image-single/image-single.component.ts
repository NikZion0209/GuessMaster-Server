import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-image-single',
  templateUrl: './image-single.component.html',
  styleUrl: './image-single.component.css'
})
export class ImageSingleComponent {
  @Input() image: { url: string, word: string } | null = null;
  
}
