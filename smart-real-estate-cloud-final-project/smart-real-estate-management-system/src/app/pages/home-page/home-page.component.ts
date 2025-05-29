import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { LoginService } from '../../services/identity/login.service';
import { Observable } from 'rxjs';
import { CommonModule } from '@angular/common';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatButtonModule } from '@angular/material/button';
// Importul NavbarComponent
import { NavbarComponent } from "../../components/navbar/navbar.component";
import { NavbarHomeComponent } from "../../components/navbar-home/navbar-home.component";

@Component({
  selector: 'app-home',
  standalone: true,
  // Adaugi NavbarComponent în array-ul imports
  imports: [
    CommonModule,
    MatSidenavModule,
    MatToolbarModule,
    MatButtonModule,
    NavbarComponent,
    NavbarHomeComponent
],
  templateUrl: './home-page.component.html',
  styleUrls: ['./home-page.component.css'],
})
export class HomePageComponent {
  constructor(
    private router: Router,
    private loginService: LoginService
  ) {}

  isLogged(): Observable<boolean> {
    return this.loginService.isAuthenticated();
  }

  navigateToProperties() {
    this.router.navigate(['properties']);
  }

  navigateToRegister() {
    this.router.navigate(['auth/register']);
  }

  navigateToLogin() {
    this.router.navigate(['auth/login']);
  }

  logout() {
    this.loginService.logout();
  }
}
