import { Component, OnInit } from '@angular/core';
import { Router, NavigationEnd } from '@angular/router';
import { filter } from 'rxjs/operators';

import { AudioService } from '../services/audio.service';
import { TimerService } from '../services/timer.service';

@Component({
    selector: 'selection-menu',
    templateUrl: './selection-menu.component.html',
    styleUrls: ['./selection-menu.component.css'],
    standalone: false
})

export class SelectionMenuComponent implements OnInit {
  showNavigationBar = true;
  showReturnButton = false;

  constructor(
    private router: Router
  ) { }

  ngOnInit() {
    this.router.events.pipe(
      filter((event) => event instanceof NavigationEnd)
    ).subscribe((event) => {
      const currentRoute = this.router.url;

      if (currentRoute !== '/') {
        this.showNavigationBar = false;
        this.showReturnButton = true;
      } else {
        this.showNavigationBar = true;
        this.showReturnButton = false;
      }
    });
  }

  navItems = [
  { label: 'Wordy Wonders', route: [{ outlets: { game: ['wordyWonders'] } }] },
  { label: 'Flag Whiz', route: [{ outlets: { game: ['flagWhiz'] } }] },
  { label: 'Word Snap', route: [{ outlets: { game: ['wordSnap'] } }] },
  { label: 'Text Blitz', route: [{ outlets: { game: ['textBlitz'] } }] }
];

wrapLetters(text: string): string {
  return text.split('').map(char =>
    char === ' ' ? '&nbsp;' : `<span>${char}</span>`
  ).join('');
}
}