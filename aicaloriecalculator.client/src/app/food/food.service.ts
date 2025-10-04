import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { map } from 'rxjs/operators';

@Injectable({ providedIn: 'root' })
export class FoodService {
  constructor(private http: HttpClient) {}

  analyzeImage(imageData: string | ArrayBuffer | null): Observable<string> {
    if (!imageData || typeof imageData !== 'string') {
      return of('No image data');
    }
    const token = localStorage.getItem('token');
    const headers = token
      ? new HttpHeaders({ Authorization: `Bearer ${token}` })
      : new HttpHeaders();
    return this.http.post<any>('/api/food/analyze', { imageBase64: imageData }, { headers }).pipe(
      map(response => response.result || 'Could not identify food.')
    );
  }
}
