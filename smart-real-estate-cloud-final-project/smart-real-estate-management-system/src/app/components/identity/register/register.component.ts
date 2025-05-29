import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { RegisterService } from '../../../services/identity/register.service';
import { CommonModule } from '@angular/common';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { faArrowLeft } from '@fortawesome/free-solid-svg-icons';

@Component({
  selector: 'app-register',
  imports: [ReactiveFormsModule, CommonModule, FontAwesomeModule],
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {
  registerForm: FormGroup;
  errorMessage: string | null = null;
  successMessage: string | null = null;
  faArrowLeft = faArrowLeft;

  constructor(
    private fb: FormBuilder,
    private registerService: RegisterService
  ) {
    this.registerForm = this.fb.group(
      {
        email: ['', [Validators.required, Validators.email]],
        password: ['', [Validators.required, Validators.minLength(8)]],
        confirmPassword: ['', [Validators.required, Validators.minLength(8)]]
      },
      { validator: this.passwordMatchValidator } // Validare personalizată
    );
  }

  ngOnInit(): void {}

  passwordMatchValidator(form: FormGroup): { [key: string]: boolean } | null {
    const password = form.get('password');
    const confirmPassword = form.get('confirmPassword');

    if (password && confirmPassword && password.value !== confirmPassword.value) {
      return { mustMatch: true };
    }
    return null;
  }

  onSubmit(): void {
    this.errorMessage = null;
    this.successMessage = null;

    if (this.registerForm.valid) {
      const { email, password } = this.registerForm.value;

      this.registerService.register(email, password).subscribe({
        next: (response) => {
          this.successMessage = 'Registration successful!';
          this.clearMessagesAfterTimeout();
          this.registerForm.reset(); // Resetează formularul după înregistrare
        },
        error: (error) => {
          this.errorMessage = error.error?.message || 'Registration failed. Please try again.';
          this.clearMessagesAfterTimeout();
        }
      });
    } else {
      this.errorMessage = 'Please fix validation errors before submitting.';
      this.clearMessagesAfterTimeout();
    }
  }

  // Clear messages after 10 seconds
  private clearMessagesAfterTimeout(): void {
    setTimeout(() => {
      this.successMessage = null;
      this.errorMessage = null;
    }, 10000);
  }
}
