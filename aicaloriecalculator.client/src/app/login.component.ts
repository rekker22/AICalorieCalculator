import { Component } from '@angular/core';
import { AuthService, LoginRequest } from './auth.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-login',
  template: `
    <div class="container mt-5" style="max-width: 400px;">
      <h2 class="text-center mb-4" style="color: #6f42c1;">Login</h2>
      <form (ngSubmit)="onSubmit()" class="bg-white p-4 rounded shadow">
        <div class="mb-3">
          <label for="email" class="form-label">Email</label>
          <input type="email" [(ngModel)]="email" name="email" id="email" class="form-control" required>
        </div>
        <div class="mb-3">
          <label for="password" class="form-label">Password</label>
          <input type="password" [(ngModel)]="password" name="password" id="password" class="form-control" required>
        </div>
        <button type="submit" class="btn w-100" style="background-color: #6f42c1; color: white;">Login</button>
      </form>
      <div *ngIf="error" class="alert alert-danger mt-3 text-center">{{error}}</div>
    </div>
  `
})
export class LoginComponent {
  email = '';
  password = '';
  error = '';

  constructor(private auth: AuthService, private router: Router) {}

  onSubmit() {
    const data: LoginRequest = { email: this.email, password: this.password };
    this.auth.login(data).subscribe({
      next: res => {
        if (res.success && res.token) {
          localStorage.setItem('token', res.token);
          // Reload the home page to update login state in AppComponent
          this.router.navigate(['/']).then(() => window.location.reload());
        } else {
          this.error = res.message || 'Login failed';
        }
      },
      error: err => {
        this.error = err.error?.message || 'Login failed';
      }
    });
  }
}
