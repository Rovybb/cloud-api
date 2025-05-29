import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Property } from '../models/property.model';
import { LoginService } from './identity/login.service';
import { environment } from '../../environments/environment';

  @Injectable({
    providedIn: 'root'
  })
  export class PropertyService {

    //private apiURL = 'https://smartrealestatemanagementsystem20250527175616.azurewebsites.net/api/v1/Properties';
    private apiURL = environment.API_URL + '/Properties';
    private inqURL = environment.API_URL + '/Inquiries'
    constructor(private http: HttpClient) { }

  public getProperties(): Observable<Property[]> {
    return this.http.get<Property[]>(this.apiURL);
  }

  public sendInquiry(
    propertyId: string,
    message: string,
    status: number,
    agentId: string,
    clientId: string
  ): Observable<any> {
    const body = {
      propertyId: propertyId,
      message: message,
      status: status,
      agentId: agentId,
      clientId: clientId
    };

      const token = this.getTokenFromCookies();
      // Setează header-ul cu token-ul JWT
      const headers = { Authorization: `Bearer ${token}` };
  
      return this.http.post(this.inqURL, body, { headers });
  }

  public getPropertiesWithPagination(
    pageNumber: number,
    pageSize: number,
    filters: { [key: string]: string | number }
  ): Observable<any> {
    let params = new HttpParams()
      .set('pageNumber', pageNumber.toString())
      .set('pageSize', pageSize.toString());
  
    for (const key in filters) {
      if (filters[key] !== null) {
        params = params.set(key, filters[key].toString());
      }
    }
  
    const token = this.getTokenFromCookies();
    const headers = { Authorization: `Bearer ${token}` };
  
    return this.http.get<any>(this.apiURL, { params, headers });
  }

  private getTokenFromCookies(): string {
    const name = 'token=';
    const decodedCookie = decodeURIComponent(document.cookie);
    const ca = decodedCookie.split(';');
    for (let i = 0; i < ca.length; i++) {
      let c = ca[i];
      while (c.charAt(0) === ' ') {
        c = c.substring(1);
      }
      if (c.indexOf(name) === 0) {
        return c.substring(name.length, c.length);
      }
    }
    return '';
  }

  public createProperty(property: Property): Observable<any> {
    const token = this.getTokenFromCookies();
    const headers = { Authorization: `Bearer ${token}` };
    return this.http.post<Property>(this.apiURL, property, { headers });
  }

  public updateProperty(propertyId: string | null, property: Property): Observable<any> {
    const token = this.getTokenFromCookies();
    const headers = { Authorization: `Bearer ${token}` };
    const url = `${this.apiURL}/${propertyId}`;
    return this.http.put<Property>(url, property, { headers });
  }

  public deleteProperty(propertyId: string): Observable<any> {
    const token = this.getTokenFromCookies();
    const headers = { Authorization: `Bearer ${token}` };
    const url = `${this.apiURL}/${propertyId}`;
    return this.http.delete(url, { headers });
  }

  public getPropertyById(propertyId: string): Observable<Property> {
    const token = this.getTokenFromCookies();
    const headers = { Authorization: `Bearer ${token}` };
    const url = `${this.apiURL}/${propertyId}`;
    return this.http.get<Property>(url, { headers });
  }

  getPropertyImages(propertyId: string): Observable<any[]> {
    const url = `${this.apiURL}/${propertyId}/images`;
    const token = this.getTokenFromCookies();
    const headers = { Authorization: `Bearer ${token}` };
    return this.http.get<any[]>(url, { headers });
  }
  
  uploadImage(propertyId: string, formData: FormData): Observable<any> {
    const url = `${this.apiURL}/${propertyId}/images`;
    const token = this.getTokenFromCookies();
    const headers = { Authorization: `Bearer ${token}` }; // Doar Authorization
  
    return this.http.post<any>(url, formData, { headers });
  }
  
  createCheckoutSession(body: any): Observable<any> {
    const url = 'https://localhost:7146/api/v1/Payments/create-checkout-session';
    const token = this.getTokenFromCookies();
    const headers = { Authorization: `Bearer ${token}` };
  
    return this.http.post<any>(url, body, { headers });
  }

  predictPrice(body: any): Observable<any> {
    const url = 'https://localhost:7146/api/v1/PropertyPricePrediction/predict';
    const token = this.getTokenFromCookies();
    const headers = { Authorization: `Bearer ${token}` };
  
    return this.http.post<any>(url, body, { headers });
  }
  
}