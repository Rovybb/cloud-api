import { TestBed } from '@angular/core/testing';
import { RegisterService } from './register.service';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { Router } from '@angular/router';
import { environment } from '../../../environments/environment';

describe('RegisterService', () => {
  let service: RegisterService;
  let httpMock: HttpTestingController;
  let routerMock: any;

  beforeEach(() => {
    routerMock = {
      navigate: jasmine.createSpy('navigate')
    };

    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [
        RegisterService,
        { provide: Router, useValue: routerMock }
      ]
    });

    service = TestBed.inject(RegisterService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should send registration request and navigate to home', () => {
    const email = 'test@example.com';
    const password = 'password123';
    const mockResponse = { success: true };

    service.register(email, password).subscribe((response) => {
      expect(response).toEqual(mockResponse);
    });

    const req = httpMock.expectOne(`${environment.API_URL}/Auth/register`);
    expect(req.request.method).toBe('POST');
    expect(req.request.body).toEqual({ email, password });

    req.flush(mockResponse);

    expect(routerMock.navigate).toHaveBeenCalledWith(['/']);
  });

  it('should handle HTTP errors gracefully', () => {
    const email = 'test@example.com';
    const password = 'password123';

    service.register(email, password).subscribe({
      next: () => fail('Expected an error, but got a success response'),
      error: (error) => {
        expect(error.status).toBe(500);
      }
    });

    const req = httpMock.expectOne(`${environment.API_URL}/Auth/register`);
    expect(req.request.method).toBe('POST');

    req.flush('Registration failed', { status: 500, statusText: 'Internal Server Error' });
    expect(routerMock.navigate).not.toHaveBeenCalled();
  });
});
