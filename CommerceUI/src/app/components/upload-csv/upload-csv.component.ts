import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CommerceService } from '../../services/commerce.service';

@Component({
  selector: 'app-upload-csv',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './upload-csv.component.html'
})
export class UploadCsvComponent {
  selectedFile: File | null = null;
  loading = false;
  successMessage = '';
  errorMessage = '';

  constructor(private commerceService: CommerceService) {}

  onFileSelected(event: any): void {
    const file: File = event.target.files[0];
    if (file) {
      if (!file.name.endsWith('.csv')) {
        this.errorMessage = 'Por favor seleccione un archivo con formato .csv';
        this.selectedFile = null;
        return;
      }
      this.selectedFile = file;
      this.errorMessage = '';
      this.successMessage = '';
    }
  }

  onUpload(): void {
    if (!this.selectedFile) {
      this.errorMessage = 'Debe seleccionar un archivo CSV primero.';
      return;
    }

    this.loading = true;
    this.errorMessage = '';
    this.successMessage = '';

    this.commerceService.uploadCsv(this.selectedFile).subscribe({
      next: (res) => {
        this.loading = false;
        this.successMessage = `${res.message} (Total registros: ${res.totalRecords})`;
        this.selectedFile = null;
      },
      error: (err) => {
        this.loading = false;
        this.errorMessage = err.error?.message || err.error?.errorDetail || 'Ocurrió un error al cargar el archivo.';
      }
    });
  }
}