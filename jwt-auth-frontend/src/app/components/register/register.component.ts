import { Component, inject } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../../services/auth.service';
import { RegisterRequest } from '../../models/auth.models';

@Component({
  selector: 'app-register',
  imports: [FormsModule, RouterLink],
  templateUrl: './register.component.html',
  styleUrl: './register.component.css'
})
export class RegisterComponent {
  private authService = inject(AuthService);
  private router = inject(Router);

  model: RegisterRequest = {
    firstName: '',
    lastName: '',
    username: '',
    password: '',
    confirmPassword: '',
    role: 'User' // Default role
  };

  isLoading = false;
  errorMessage = '';

  onSubmit() {
    this.errorMessage = '';

    // Validation
    if (!this.model.firstName || !this.model.lastName || !this.model.username || !this.model.password) {
      this.errorMessage = 'Please complete all required fields.';
      return;
    }

    if (this.model.password !== this.model.confirmPassword) {
      this.errorMessage = 'Password and Confirm Password do not match!';
      return;
    }

    this.isLoading = true;

    this.authService.register(this.model).subscribe({
      next: (res) => {
        this.isLoading = false;
        // Redirect to login with username prefilled
        this.router.navigate(['/login'], {
          queryParams: { registered: this.model.username }
        });
      },
      error: (err) => {
        this.isLoading = false;
        console.error('Registration error:', err);
        if (err.error && err.error.message) {
          this.errorMessage = err.error.message;
        } else if (err.status === 0) {
          this.errorMessage = `Cannot connect to backend at ${this.authService.apiUrl()}. Ensure .NET is running.`;
        } else {
          this.errorMessage = err.message || 'Registration failed. Check server console.';
        }
      }
    });
  }
}
