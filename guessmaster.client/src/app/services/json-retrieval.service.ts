import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})

export class JsonRetrievalService {
  constructor(private http: HttpClient) {}

  getObjects(jsonUrl : string): Observable<{ url: string, word: string }[]> {
    return this.http.get<{ url: string, word: string }[]>(jsonUrl);
  }
}
