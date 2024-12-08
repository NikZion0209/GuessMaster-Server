import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AnimalService {
  private jsonUrl = '../../assets/json/animals.json';

  constructor(private http: HttpClient) {}

  getAnimals(): Observable<{ url: string, word: string }[]> {
    return this.http.get<{ url: string, word: string }[]>(this.jsonUrl);
  }
}
