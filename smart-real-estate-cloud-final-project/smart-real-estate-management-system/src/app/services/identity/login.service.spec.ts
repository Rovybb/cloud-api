import { TestBed } from '@angular/core/testing';
import { LoginService } from './login.service';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { Router } from '@angular/router';
import { environment } from '../../../environments/environment';

describe('LoginService', () => {
  let service: LoginService;
  let httpMock: HttpTestingController;
  let routerMock: any;

  beforeEach(() => {
    routerMock = {
      navigate: jasmine.createSpy('navigate')
    };

    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [
        LoginService,
        { provide: Router, useValue: routerMock }
      ]
    });

    service = TestBed.inject(LoginService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
    document.cookie = 'token=; expires=Thu, 01 Jan 1970 00:00:00 UTC; path=/;';
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  describe('login', () => {
    it('should send login request and set token on success', () => {
      const email = 'test@example.com';
      const password = 'password123';
      const mockResponse = { token: 'mockToken123' };

      service.login(email, password).subscribe((response) => {
        expect(response).toEqual(mockResponse);
      });

      const req = httpMock.expectOne(`${environment.API_URL}/Auth/login`);
      expect(req.request.method).toBe('POST');
      expect(req.request.body).toEqual({ email, password });

      req.flush(mockResponse);

      expect(document.cookie).toContain('token=mockToken123');
      expect(routerMock.navigate).toHaveBeenCalledWith(['/']);
      expect(service.isAuthenticatedSubject.value).toBeTrue();
    });

    it('should handle login error gracefully', () => {
      const email = 'test@example.com';
      const password = 'password123';

      service.login(email, password).subscribe({
        next: () => fail('Expected an error, but got a success response'),
        error: (error) => {
          expect(error.status).toBe(401);
        }
      });

      const req = httpMock.expectOne(`${environment.API_URL}/Auth/login`);
      expect(req.request.method).toBe('POST');

      req.flush('Login failed', { status: 401, statusText: 'Unauthorized' });

      expect(document.cookie).not.toContain('token=');
      expect(routerMock.navigate).not.toHaveBeenCalled();
      expect(service.isAuthenticatedSubject.value).toBeFalse();
    });
  });

  describe('isAuthenticated', () => {
    it('should return authentication state as Observable', (done) => {
      service.isAuthenticated().subscribe((isAuthenticated) => {
        expect(isAuthenticated).toBeFalse();
        done();
      });
    });
  });

  describe('logout', () => {
    it('should clear token and navigate to home', () => {
      document.cookie = 'token=mockToken123; path=/;';
      service.setAuthenticationState(true);

      service.logout();

      expect(document.cookie).not.toContain('token=');
      expect(service.isAuthenticatedSubject.value).toBeFalse();
      expect(routerMock.navigate).toHaveBeenCalledWith(['/']);
    });
  });
});
