import { TestBed } from '@angular/core/testing';

import { JsonRetrievalService } from './json-retrieval.service';

describe('JsonRetrievalService', () => {
  let service: JsonRetrievalService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(JsonRetrievalService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
