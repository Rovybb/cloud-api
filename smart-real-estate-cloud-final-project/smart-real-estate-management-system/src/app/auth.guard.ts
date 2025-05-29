import { Injectable } from '@angular/core';
import { CanActivate, Router } from '@angular/router';
import { LoginService } from './services/identity/login.service';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class AuthGuard implements CanActivate {

  constructor(private loginService: LoginService, private router: Router) {}

  canActivate(): Observable<boolean> {
    return this.loginService.isAuthenticated().pipe(
      map(isAuthenticated => {
        if (!isAuthenticated) {
          this.router.navigate(['/auth/login']); // Redirect to login if not authenticated
          return false;
        }
        return true;
      })
    );
  }
}
