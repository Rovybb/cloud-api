// profile-page.component.ts
import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { LoginService } from '../../services/identity/login.service';
import { UserInformationService, UserInformation } from '../../services/user-information.service';
import { CommonModule } from '@angular/common';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatButtonModule } from '@angular/material/button';
import { NavbarComponent } from "../../components/navbar/navbar.component";
import { Observable } from 'rxjs';
import { MatDialog, MatDialogModule } from '@angular/material/dialog'; // Import MatDialog
import { UpdateUserDialogComponent } from '../../components/update-user-dialog/update-user-dialog.component';
import { NavbarHomeComponent } from "../../components/navbar-home/navbar-home.component"; // Adjust the path as needed

@Component({
  selector: 'app-profile-page',
  standalone: true,
  imports: [
    CommonModule,
    MatSidenavModule,
    MatToolbarModule,
    MatButtonModule,
    MatDialogModule, // Import MatDialogModule
],
  templateUrl: './profile-page.component.html',
  styleUrls: ['./profile-page.component.css'],
})
export class ProfilePageComponent implements OnInit {
  user: UserInformation | null = null;
  isAuthenticated: boolean = false;
  isLoading: boolean = true;
  error: string | null = null;

  constructor(
    private router: Router,
    private loginService: LoginService,
    private userInformationService: UserInformationService,
    private dialog: MatDialog // Inject MatDialog
  ) {}

  ngOnInit(): void {
    // Subscribe to authentication status
    this.loginService.isAuthenticated$.subscribe(isAuth => {
      this.isAuthenticated = isAuth;
      if (isAuth) {
        this.fetchUserInformation();
      } else {
        this.user = null;
        this.isLoading = false;
      }
    });
  }

  // Fetch user information using the user ID from the token
  private fetchUserInformation(): void {
    const userId = this.loginService.getUserId();
    if (userId) {
      this.userInformationService.getUserById(userId).subscribe(
        (userInfo) => {
          this.user = userInfo;
          this.isLoading = false;
        },
        (error) => {
          console.error('Error fetching user information:', error);
          this.error = 'Failed to load user information.';
          this.isLoading = false;
        }
      );
    } else {
      console.error('User ID not found in token.');
      this.error = 'Invalid authentication token.';
      this.isLoading = false;
    }
  }

  navigateToProperties() {
    this.router.navigate(['properties']);
  }

  navigateToRegister() {
    this.router.navigate(['auth/register']);
  }

  isLogged(): Observable<boolean> {
    return this.loginService.isAuthenticated();
  }

  navigateToLogin() {
    this.router.navigate(['auth/login']);
  }

  logout() {
    this.loginService.logout();
  }

  openUpdateDialog(): void {
    if (!this.user) {
      return;
    }

    const dialogRef = this.dialog.open(UpdateUserDialogComponent, {
      width: '500px',
      data: this.user, // Pass current user data to the dialog, including id
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        // If the dialog was closed after a successful update, refresh user data
        this.fetchUserInformation();
      }
    });
  }


  getUserStatus(status: any): string {
    switch (status) {
      case 0:
        return 'active';
      case 1:
        return 'inactive';
      case 2:
        return 'banned';
      default:
        return 'unknown'; // În caz de status invalid
    }
  }
  getUserRole(role: any): string {
    switch (role) {
      case 0:
        return 'client';
      case 1:
        return 'proffesional';
      default:
        return 'unknown'; // În caz de rol invalid
    }
  }
}
