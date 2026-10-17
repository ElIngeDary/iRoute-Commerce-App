import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './login.component.html'
})
export class LoginComponent {
  username = '';
  password = '';
  errorMessage = '';

  constructor(private authService: AuthService) {}

  onSubmit(): void {
    if (!this.username.trim() || !this.password.trim()) {
      this.errorMessage = 'Por favor ingrese usuario y contraseña.';
      return;
    }

    if (this.username === 'admin' && this.password === '123456') {
      this.authService.login(this.username);
    } else {
      this.errorMessage = 'Credenciales inválidas. Usa: admin / 123456';
    }
  }
}