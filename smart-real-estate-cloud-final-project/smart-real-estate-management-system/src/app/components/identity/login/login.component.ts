import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { faArrowLeft } from '@fortawesome/free-solid-svg-icons';
import { LoginService } from '../../../services/identity/login.service';

@Component({
  selector: 'app-login',
  imports: [ReactiveFormsModule, CommonModule, FontAwesomeModule],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css'
})
export class LoginComponent implements OnInit {
  loginForm: FormGroup;
  errorMessage: string | null = null;
  faArrowLeft = faArrowLeft;

  constructor(
    private fb: FormBuilder,
    private loginService: LoginService
  ) {
    this.loginForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(8)]]
    });
  }

  ngOnInit(): void {}

  onSubmit(): void {
    this.errorMessage = null;

    if (this.loginForm.valid) {
      const { email, password } = this.loginForm.value;
      this.loginService.login(email, password).subscribe({
        next: () => {
          this.loginService.setAuthenticationState(true);
          console.log('User logged in');
        },
        error: (error) => {
          this.errorMessage = 'Login failed. Please try again.';
          console.error('Login error:', error);
        }
      });
    } else {
      this.errorMessage = 'Please fix validation errors before submitting.';
    }
  }

}
