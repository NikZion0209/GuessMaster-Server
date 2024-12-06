import { Component, OnInit } from '@angular/core';
import { Router, NavigationEnd } from '@angular/router';
import { filter } from 'rxjs/operators';

@Component({
  selector: 'selection-menu',
  templateUrl: './selection-menu.component.html',
  styleUrls: ['./selection-menu.component.css']
})

export class SelectionMenuComponent implements OnInit {
  showNavigationBar = true;
  showReturnButton = false;

  constructor(private router: Router) { }

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
}