import { Component, Input, Output, EventEmitter, OnInit } from '@angular/core';

@Component({
  selector: 'app-word-list',
  templateUrl: './word-list.component.html',
  styleUrls: ['./word-list.component.css']
})
export class WordListComponent implements OnInit {
  @Input() words: { text: string, disabled: boolean }[] = [];
  @Output() wordSelected = new EventEmitter<{ word: { text: string, disabled: boolean }, mouse: MouseEvent }>();

  ngOnInit() {
    this.shuffleWords();
  }

  shuffleWords() {
    for (let i = this.words.length - 1; i > 0; i--) {
      const j = Math.floor(Math.random() * (i + 1));
      [this.words[i], this.words[j]] = [this.words[j], this.words[i]];
    }
  }

  selectWord(word: { text: string, disabled: boolean }, mouse: MouseEvent) {
    this.wordSelected.emit({ word, mouse });
  }
}
