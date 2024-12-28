import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ScoreService {
  private scoreSubject = new BehaviorSubject<number>(0);
  score$ = this.scoreSubject.asObservable();

  addScore(points: number) {
    this.scoreSubject.next(this.scoreSubject.value + points);
  }

  retrieveScore() {
    return this.scoreSubject.value;
  }
  
  resetScore() {
    this.scoreSubject.next(0);
  }
}
