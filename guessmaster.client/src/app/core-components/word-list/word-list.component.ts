import { Component, Input, Output, EventEmitter } from '@angular/core';

@Component({
  selector: 'app-word-list',
  templateUrl: './word-list.component.html',
  styleUrls: ['./word-list.component.css']
})

export class WordListComponent  {
  @Input() words: { text: string, disabled: boolean }[] = [];
  @Output() wordSelected = new EventEmitter<{ word: { text: string, disabled: boolean }, mouse: MouseEvent }>();

  selectWord(word: { text: string, disabled: boolean }, mouse: MouseEvent) {
    this.wordSelected.emit({ word, mouse });
  }
}
