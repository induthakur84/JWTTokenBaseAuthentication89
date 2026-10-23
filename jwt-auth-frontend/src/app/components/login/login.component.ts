import { Component, inject, OnInit } from '@angular/core';
import { Router, ActivatedRoute, RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../../services/auth.service';
import { LoginRequest } from '../../models/auth.models';

@Component({
  selector: 'app-login',
  imports: [FormsModule, RouterLink],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css'
})
export class LoginComponent implements OnInit {
  private authService = inject(AuthService);
  private router = inject(Router);
  private route = inject(ActivatedRoute);

  credentials: LoginRequest = {
    username: '',
    password: ''
  };

  isLoading = false;
  errorMessage = '';

  ngOnInit(): void {
    if (this.authService.isLoggedIn()) {
      this.router.navigate(['/products']);
    }

    const registeredUser = this.route.snapshot.queryParams['registered'];
    if (registeredUser) {
      this.credentials.username = registeredUser;
    }
  }

  // Quick fill helper for classroom practical demo
  fillDemo(role: 'Admin' | 'Employee' | 'User'): void {
    if (role === 'Admin') {
      this.credentials = { username: 'payal123@gmail.com', password: '123456' };
    } else if (role === 'Employee') {
      this.credentials = { username: 'employee', password: 'Password123!' };
    } else {
      this.credentials = { username: 'student_user', password: 'Password123!' };
    }
  }

  onSubmit(): void {
    if (!this.credentials.username || !this.credentials.password) {
      this.errorMessage = 'Please enter username and password.';
      return;
    }

    this.isLoading = true;
    this.errorMessage = '';

    this.authService.login(this.credentials).subscribe({
      next: () => {
        this.isLoading = false;
        const returnUrl = this.route.snapshot.queryParams['returnUrl'] || '/products';
        this.router.navigateByUrl(returnUrl);
      },
      error: (err) => {
        this.isLoading = false;
        if (err.status === 0) {
          this.errorMessage = 'Cannot connect to backend. Is the ASP.NET Core API running?';
        } else {
          this.errorMessage = err.error?.message || err.error?.error || 'Invalid username or password.';
        }
      }
    });
  }
}
