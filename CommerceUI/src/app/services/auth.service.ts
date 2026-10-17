import { Injectable } from '@angular/core';
import { Router } from '@angular/router';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private readonly AUTH_KEY = 'iroute_auth_user';

  constructor(private router: Router) { }

  login(username: string): void {
    localStorage.setItem(this.AUTH_KEY, JSON.stringify({ username, token: 'fake-jwt-token-iroute' }));
    this.router.navigate(['/upload']);
  }

  logout(): void {
    localStorage.removeItem(this.AUTH_KEY);
    this.router.navigate(['/login']);
  }

  isAuthenticated(): boolean {
    return localStorage.getItem(this.AUTH_KEY) !== null;
  }

  getUserName(): string {
    const userStr = localStorage.getItem(this.AUTH_KEY);
    if (!userStr) return '';
    const user = JSON.parse(userStr);
    return user.username || '';
  }
}