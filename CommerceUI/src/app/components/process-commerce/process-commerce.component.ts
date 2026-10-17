import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { CommerceService } from '../../services/commerce.service';

@Component({
  selector: 'app-process-commerce',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './process-commerce.component.html'
})
export class ProcessCommerceComponent {
  processDate: string = '2026-10-16';
  loading = false;
  successMessage = '';
  errorMessage = '';

  constructor(private commerceService: CommerceService) {}

  onProcess(): void {
    if (!this.processDate) {
      this.errorMessage = 'Por favor seleccione una fecha válida.';
      return;
    }

    this.loading = true;
    this.errorMessage = '';
    this.successMessage = '';

    this.commerceService.processCommerce(this.processDate).subscribe({
      next: (res) => {
        this.loading = false;
        this.successMessage = res.message;
      },
      error: (err) => {
        this.loading = false;
        this.errorMessage = err.error?.message || 'Error al ejecutar el procesamiento por fecha.';
      }
    });
  }
}