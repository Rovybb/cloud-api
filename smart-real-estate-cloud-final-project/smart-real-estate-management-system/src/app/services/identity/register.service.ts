import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, tap } from 'rxjs';
import { Router } from '@angular/router';  // Importă Router
import { environment } from '../../../environments/environment';

interface RegisterPayload {
  email: string;
  password: string;
}

@Injectable({
  providedIn: 'root'
})
export class RegisterService {

  private apiUrl = environment.API_URL + '/Auth/register';

  constructor(private http: HttpClient, private router: Router) { }  // Injectează Router

  // Metodă pentru a trimite datele de înregistrare la API
  register(email: string, password: string): Observable<any> {
    const payload: RegisterPayload = { email, password };
    return this.http.post(this.apiUrl, payload).pipe(
      tap(() => {
        this.router.navigate(['/']);  // Redirecționează utilizatorul pe Home după înregistrare
      })
    );
  }
}
