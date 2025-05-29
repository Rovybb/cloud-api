import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { PropertyService } from '../../services/property.service';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule } from '@angular/forms';
import {jwtDecode} from 'jwt-decode';
import { NavbarHomeComponent } from '../navbar-home/navbar-home.component';

@Component({
  selector: 'app-property-create',
  standalone: true,
  imports: [ReactiveFormsModule, CommonModule, NavbarHomeComponent],
  templateUrl: './property-create.component.html',
  styleUrls: ['./property-create.component.css']
})
export class PropertyCreateComponent implements OnInit {
  propertyForm: FormGroup;
  errorMessage: string | null = null;

  constructor(
    private fb: FormBuilder,
    private propertyService: PropertyService,
    private router: Router
  ) {
    this.propertyForm = this.fb.group({
      title: ['', [Validators.required, Validators.maxLength(100)]],
      description: ['', [Validators.required, Validators.maxLength(500)]],
      status: ['', [Validators.required]],
      type: ['', [Validators.required]],
      price: [null, [Validators.required, Validators.min(0.01)]],
      address: ['', [Validators.required, Validators.maxLength(200)]],
      area: [null, [Validators.required, Validators.min(0.01)]],
      rooms: [null, [Validators.required, Validators.min(0)]],
      bathrooms: [null, [Validators.required, Validators.min(0)]],
      constructionYear: [null, [Validators.required, Validators.min(1501)]],
    });
  }

  ngOnInit(): void {
    const token = this.getTokenFromCookie('token');
    if (token) {
      const decodedToken: any = jwtDecode(token);
      const userId = decodedToken['unique_name'];
      if (userId) {
        this.propertyForm.addControl('userId', this.fb.control(userId, Validators.required));
        console.log('User ID:', userId);
      } else {
        this.errorMessage = 'User ID not found in token.';
      }
    } else {
      this.errorMessage = 'Authentication token not found.';
    }
  }

  onSubmit(): void {
    this.errorMessage = null;

    if (this.propertyForm.valid) {
      this.propertyService.createProperty(this.propertyForm.value).subscribe({
        next: () => {
          this.router.navigate(['/properties']);
        },
        error: (error) => {
          this.errorMessage = 'Error creating property. Please try again.';
          console.error('Create error:', error);
        }
      });
    } else {
      this.errorMessage = 'Please fix validation errors before submitting.';
    }
  }

  private getTokenFromCookie(cookieName: string): string | null {
    const cookies = document.cookie.split('; ');
    for (const cookie of cookies) {
      const [name, value] = cookie.split('=');
      if (name === cookieName) {
        return value;
      }
    }
    return null;
  }
}
