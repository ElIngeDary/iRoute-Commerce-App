import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { CommerceQuarantineDto, ProcessResponseDto, UploadResponseDto } from '../models/commerce.model';

@Injectable({
  providedIn: 'root'
})
export class CommerceService {
  // Ruta base de la API .NET 8
  private readonly apiUrl = 'http://localhost:5107/api/Commerce';

  constructor(private http: HttpClient) { }

  /**
   * Envía un archivo CSV mediante multipart/form-data al backend
   */
  uploadCsv(file: File): Observable<UploadResponseDto> {
    const formData = new FormData();
    formData.append('file', file, file.name);

    return this.http.post<UploadResponseDto>(`${this.apiUrl}/upload`, formData);
  }

  /**
   * Ejecuta el proceso de validación y filtrado por fecha
   */
  processCommerce(processDate: string): Observable<ProcessResponseDto> {
    return this.http.post<ProcessResponseDto>(`${this.apiUrl}/process`, { processDate });
  }

  /**
   * Obtiene el listado completo de registros que fueron movidos a cuarentena
   */
  getQuarantineList(): Observable<CommerceQuarantineDto[]> {
    return this.http.get<CommerceQuarantineDto[]>(`${this.apiUrl}/quarantine`);
  }
}