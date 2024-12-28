import { Component, Input, Output, EventEmitter } from '@angular/core';

@Component({
  selector: 'app-word-options',
  templateUrl: './word-options.component.html',
  styleUrl: './word-options.component.css'
})
export class WordOptionsComponent {
  @Input() words: { text: string }[] = [];
  @Output() wordSelected = new EventEmitter<{ word: { text: string } }>();
  
  selectWord(word: { text: string }) {
    this.wordSelected.emit({ word });
  }
}
