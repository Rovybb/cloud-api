// update-user-dialog.component.ts
import { Component, Inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef, MatDialogModule } from '@angular/material/dialog';
import { UserInformationService, UserInformation } from '../../services/user-information.service';
import { MatButtonModule } from '@angular/material/button';
import { MatInputModule } from '@angular/material/input';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-update-user-dialog',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    MatDialogModule,
    MatButtonModule,
    MatInputModule,
    CommonModule,
    // Add other Angular Material modules as needed
  ],
  templateUrl: './update-user-dialog.component.html',
  styleUrls: ['./update-user-dialog.component.css'],
})
export class UpdateUserDialogComponent implements OnInit {
  updateForm: FormGroup;
  isSubmitting: boolean = false;
  error: string | null = null;

  constructor(
    private fb: FormBuilder,
    private userService: UserInformationService,
    public dialogRef: MatDialogRef<UpdateUserDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: UserInformation
  ) {
    // Initialize the form with the current user data
    this.updateForm = this.fb.group({
      username: [data.username, [Validators.required]],
      email: [data.email, [Validators.required, Validators.email]],
      firstName: [data.firstName, [Validators.required]],
      lastName: [data.lastName, [Validators.required]],
      address: [data.address],
      phoneNumber: [data.phoneNumber],
      nationality: [data.nationality],
      status: [data.status, [Validators.required]],
      role: [data.role, [Validators.required]],
      company: [data.company],
      type: [data.type],
      // Note: lastLogin is typically managed by the backend and not editable by the user
    });
  }

  ngOnInit(): void {}

  onSubmit() {
    if (this.updateForm.invalid) {
      return;
    }

    this.isSubmitting = true;
    this.error = null;

    const updatedData: UserInformation = {
      ...this.data,
      ...this.updateForm.value,
      // lastLogin is optional and likely managed by the server
    };

    this.userService.updateUser(this.data.id, updatedData).subscribe(
      (response) => {
        this.isSubmitting = false;
        this.dialogRef.close(true); // Pass true to indicate success
      },
      (error) => {
        console.error('Error updating user information:', error);
        this.error = 'Failed to update user information. Please try again.';
        this.isSubmitting = false;
      }
    );
  }

  onCancel(): void {
    this.dialogRef.close(false); // Pass false to indicate cancellation
  }
}
