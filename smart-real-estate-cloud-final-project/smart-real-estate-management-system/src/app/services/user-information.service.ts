// user-information.service.ts
import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http'; // Import HttpHeaders
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

export interface UserInformation {
  id: string; // Added id field
  email: string;
  username: string;
  firstName: string;
  lastName: string;
  address: string;
  phoneNumber: string;
  nationality: string;
  lastLogin?: Date;
  status: number; // Changed to number based on JSON structure
  role: number;
  company?: string;
  type?: string;
}

@Injectable({
  providedIn: 'root',
})
export class UserInformationService {
  private apiUrl = `${environment.API_URL}/UserInformation`; // Adjusted for consistency

  constructor(private http: HttpClient) {}

  getUserById(id: string): Observable<UserInformation> {
    const token = this.getTokenFromCookies();
    const headers = { Authorization: `Bearer ${token}` };
    return this.http.get<UserInformation>(`${this.apiUrl}/${id}`, { headers });
  }

  updateUser(id: string, userData: UserInformation): Observable<any> {
    const token = this.getTokenFromCookies();
    const headers = new HttpHeaders({
      'Content-Type': 'application/json',
      Authorization: `Bearer ${token}`,
    });
    return this.http.put(`${this.apiUrl}/${id}`, userData, { headers });
  }

  private getTokenFromCookies(): string {
    const name = 'token=';
    const decodedCookie = decodeURIComponent(document.cookie);
    const ca = decodedCookie.split(';');
    for (let c of ca) {
      c = c.trim();
      if (c.startsWith(name)) {
        return c.substring(name.length, c.length);
      }
    }
    return '';
  }
}
