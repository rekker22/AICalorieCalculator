import { Component } from '@angular/core';
import { AuthService, RegistrationRequest } from './auth.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-signup',
  template: `
    <div class="container mt-5" style="max-width: 400px;">
      <h2 class="text-center mb-4" style="color: #6f42c1;">Sign Up</h2>
      <form (ngSubmit)="onSubmit()" class="bg-white p-4 rounded shadow">
        <div class="mb-3">
          <label for="email" class="form-label">Email</label>
          <input type="email" [(ngModel)]="email" name="email" id="email" class="form-control" required>
        </div>
        <div class="mb-3">
          <label for="password" class="form-label">Password</label>
          <input type="password" [(ngModel)]="password" name="password" id="password" class="form-control" required>
        </div>
        <div class="mb-3">
          <label for="confirmPassword" class="form-label">Confirm Password</label>
          <input type="password" [(ngModel)]="confirmPassword" name="confirmPassword" id="confirmPassword" class="form-control" required>
        </div>
        <div class="mb-3">
          <label for="firstName" class="form-label">First Name</label>
          <input type="text" [(ngModel)]="firstName" name="firstName" id="firstName" class="form-control">
        </div>
        <div class="mb-3">
          <label for="lastName" class="form-label">Last Name</label>
          <input type="text" [(ngModel)]="lastName" name="lastName" id="lastName" class="form-control">
        </div>
        <button type="submit" class="btn w-100" style="background-color: #6f42c1; color: white;">Sign Up</button>
      </form>
      <div *ngIf="error" class="alert alert-danger mt-3 text-center">{{error}}</div>
    </div>
  `
})
export class SignupComponent {
  email = '';
  password = '';
  confirmPassword = '';
  firstName = '';
  lastName = '';
  error = '';

  constructor(private auth: AuthService, private router: Router) {}

  onSubmit() {
    const data: RegistrationRequest = {
      email: this.email,
      password: this.password,
      confirmPassword: this.confirmPassword,
      firstName: this.firstName,
      lastName: this.lastName
    };
    this.auth.signup(data).subscribe({
      next: res => {
        if (res.success && res.token) {
          localStorage.setItem('token', res.token);
          // Reload the home page to update login state in AppComponent
          this.router.navigate(['/']).then(() => window.location.reload());
        } else {
          this.error = res.message || 'Signup failed';
        }
      },
      error: err => {
        this.error = err.error?.message || 'Signup failed';
      }
    });
  }
}
