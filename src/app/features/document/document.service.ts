import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { CreateDocumentDto, DocumentProp } from '../../model/document.type';
import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class DocumentService {
  private apiUrl = `${environment.apiUrl}/document`;

  constructor(private http: HttpClient) { }

  getDocuments(): Observable<DocumentProp[]> {
    return this.http.get<DocumentProp[]>(this.apiUrl);
  }

  getDocumentById(id: string): Observable<DocumentProp> {
    return this.http.get<DocumentProp>(`${this.apiUrl}/${id}`);
  }

  uploadDocument(formData: FormData): Observable<any> {
    return this.http.post(`${this.apiUrl}/upload`, formData);
  }

  updateDocument(formData: FormData, id: string): Observable<any> {
    return this.http.put(`${this.apiUrl}/${id}`, formData);
  }
  downloadDocument(id: string): Observable<any> {
    return this.http.get(`${this.apiUrl}/download/${id}`, {
      responseType: 'blob', // สำคัญมาก
    });
  }

  deleteDocument(id: string): Observable<any> {
    return this.http.delete(`${this.apiUrl}/${id}`);
  }

}
