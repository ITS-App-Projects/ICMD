import { TestBed } from '@angular/core/testing';

import { BulkDeleteService } from './bulk-delete.service';

describe('BulkDeleteService', () => {
  let service: BulkDeleteService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(BulkDeleteService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
