import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { PropertyService } from '../../services/property.service';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule } from '@angular/forms';

@Component({
  selector: 'app-property-update',
  standalone: true,
  imports: [ReactiveFormsModule, CommonModule],
  templateUrl: './property-update.component.html',
  styleUrls: ['./property-update.component.css']
})
export class PropertyUpdateComponent implements OnInit {
  propertyForm: FormGroup;
  errorMessage: string | null = null;
  propertyId: string | null = null;

  constructor(
    private fb: FormBuilder,
    private propertyService: PropertyService,
    private router: Router,
    private route: ActivatedRoute
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
      userId: ['', Validators.required]
    });
  }

  ngOnInit(): void {
    this.propertyId = this.route.snapshot.paramMap.get('id');
    if (this.propertyId) {
      
      this.propertyService.getPropertyById(this.propertyId).subscribe({
        next: (property) => {
          this.propertyForm.patchValue(property);
        },
        error: (error) => {
          this.errorMessage = 'Error loading property data.';
          console.error('Error loading property:', error);
        }
      });
    }
  }

  onSubmit(): void {
    this.errorMessage = null;

    if (this.propertyForm.valid) {
      this.propertyService
        .updateProperty(this.propertyId, this.propertyForm.value)
        .subscribe({
          next: () => {
            this.router.navigate(['/properties']);
          },
          error: (error) => {
            this.errorMessage = 'Error updating property. Please try again.';
            console.error('Update error:', error);
          }
        });
    } else {
      this.errorMessage = 'Please fix validation errors before submitting.';
    }
  }
}
