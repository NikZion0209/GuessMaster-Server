import { Component, Input } from '@angular/core';
import { ScoreService } from '../../services/score.service';

@Component({
  selector: 'app-score',
  templateUrl: './score.component.html',
  styleUrls: ['./score.component.css']
})
export class ScoreComponent {
  @Input() score: number = 0;

  constructor(private scoreService: ScoreService) {}
  
    ngAfterViewInit() {
      this.scoreService.score$.subscribe(score => {
        this.score = score;
      });
    }
}
