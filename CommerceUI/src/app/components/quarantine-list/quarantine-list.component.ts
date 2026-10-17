import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CommerceService } from '../../services/commerce.service';
import { CommerceQuarantineDto } from '../../models/commerce.model';

@Component({
  selector: 'app-quarantine-list',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './quarantine-list.component.html'
})
export class QuarantineListComponent implements OnInit {
  quarantineItems: CommerceQuarantineDto[] = [];
  loading = false;
  errorMessage = '';

  constructor(private commerceService: CommerceService) {}

  ngOnInit(): void {
    this.loadQuarantineList();
  }

  loadQuarantineList(): void {
    this.loading = true;
    this.errorMessage = '';

    this.commerceService.getQuarantineList().subscribe({
      next: (data) => {
        this.quarantineItems = data;
        this.loading = false;
      },
      error: (err) => {
        this.loading = false;
        this.errorMessage = 'No se pudo obtener el listado de comercios en cuarentena.';
      }
    });
  }
}