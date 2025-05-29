import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable } from 'rxjs';
import { catchError, tap } from 'rxjs/operators';
import { Router } from '@angular/router';  // Importă Router
import { environment } from '../../../environments/environment';

declare var process : {
  env: {
    API_URL: string
  }
}
@Injectable({
  providedIn: 'root'
})
export class LoginService {
  
  private apiUrl = environment.API_URL + '/Auth/login';
  public isAuthenticatedSubject: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(!!document.cookie.split('; ').find(row => row.startsWith('token='))); // default value: see if the user is logged in from cookies
  public isAuthenticated$: Observable<boolean> = this.isAuthenticatedSubject.asObservable();

  constructor(private http: HttpClient, private router: Router) { }  // Injectează Router

  // Metodă pentru logarea utilizatorului
  login(email: string, password: string): Observable<any> {
    const loginData = { email, password };

    return this.http.post<any>(this.apiUrl, loginData).pipe(
      catchError((error) => {
        console.error('Login failed', error);
        throw error;
      }),
      tap((response) => {
        if (response && response.token) {
          document.cookie = `token=${response.token}; path=/;`;
          this.setAuthenticationState(true);
          this.router.navigate(['/']);  // Redirecționează utilizatorul pe Home
        }
      })
    );
  }

  // Setează starea autentificării
  setAuthenticationState(isAuthenticated: boolean): void {
    this.isAuthenticatedSubject.next(isAuthenticated);
  }

  // Verifică dacă utilizatorul este autentificat
  isAuthenticated(): Observable<boolean> {
    return this.isAuthenticated$;
  }

  // Metodă pentru logout
  logout(): void {
    this.setAuthenticationState(false);
    document.cookie = 'token=; expires=Thu, 01 Jan 1970 00:00:00 UTC; path=/;';
    console.log('User logged out');
    this.router.navigate(['/']); // Redirecționează utilizatorul pe Home după logout
  }

  private decodeToken(token: string): any {
    try {
      const payload = token.split('.')[1]; // Partea payload din JWT
      return JSON.parse(atob(payload)); // Decodează Base64
    } catch (error) {
      console.error('Error decoding token:', error);
      return null;
    }
  }
  
  getUserId(): string | null {
    const token = this.getToken();
    console.log('Token from cookies:', token); // Adaugă acest log
    if (!token) {
      console.error('No token found!');
      return null;
    }
  
    const decoded = this.decodeToken(token);
    console.log('Decoded token payload:', decoded); // Adaugă acest log
    return decoded ? decoded.unique_name : null; // Presupunem că `userId` este prezent în payload
  }

  getToken(): string | null {
    const name = 'token=';
    const decodedCookie = decodeURIComponent(document.cookie);
    const cookies = decodedCookie.split(';');
    for (let i = 0; i < cookies.length; i++) {
      let c = cookies[i].trim();
      if (c.indexOf(name) === 0) {
        return c.substring(name.length);
      }
    }
    return null;
  }
}