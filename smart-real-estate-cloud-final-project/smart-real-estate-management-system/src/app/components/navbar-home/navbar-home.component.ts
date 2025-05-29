import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { LoginService } from '../../services/identity/login.service';

@Component({
  selector: 'app-navbar-home',
  imports: [],
  templateUrl: './navbar-home.component.html',
  styleUrl: './navbar-home.component.css'
})
export class NavbarHomeComponent {
constructor(private router: Router, private loginService: LoginService) {}
  logout() {
    this.loginService.logout();
  }
}