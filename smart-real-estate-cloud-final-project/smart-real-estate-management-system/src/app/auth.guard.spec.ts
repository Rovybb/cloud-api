import { TestBed } from '@angular/core/testing';
import { AuthGuard } from './auth.guard';
import { Router } from '@angular/router';
import { of } from 'rxjs';
import { LoginService } from './services/identity/login.service';

describe('AuthGuard', () => {
  let authGuard: AuthGuard;
  let loginServiceSpy: jasmine.SpyObj<LoginService>;
  let routerSpy: jasmine.SpyObj<Router>;

  beforeEach(() => {
    // Cream spy-uri pentru LoginService și Router
    const loginServiceMock = jasmine.createSpyObj('LoginService', ['isAuthenticated']);
    const routerMock = jasmine.createSpyObj('Router', ['navigate']);

    TestBed.configureTestingModule({
      providers: [
        AuthGuard,
        { provide: LoginService, useValue: loginServiceMock },
        { provide: Router, useValue: routerMock },
      ]
    });

    authGuard = TestBed.inject(AuthGuard);
    loginServiceSpy = TestBed.inject(LoginService) as jasmine.SpyObj<LoginService>;
    routerSpy = TestBed.inject(Router) as jasmine.SpyObj<Router>;
  });

  it('should allow activation if authenticated', (done: DoneFn) => {
    // Simulăm ca utilizatorul este autentificat
    loginServiceSpy.isAuthenticated.and.returnValue(of(true));

    authGuard.canActivate().subscribe(result => {
      expect(result).toBeTrue();
      // Nu se apelează navigarea dacă e autentificat
      expect(routerSpy.navigate).not.toHaveBeenCalled();
      done();
    });
  });

  it('should not allow activation if not authenticated and redirect to login', (done: DoneFn) => {
    // Simulăm ca utilizatorul nu este autentificat
    loginServiceSpy.isAuthenticated.and.returnValue(of(false));

    authGuard.canActivate().subscribe(result => {
      expect(result).toBeFalse();
      // Se apelează navigarea către '/auth/login'
      expect(routerSpy.navigate).toHaveBeenCalledWith(['/auth/login']);
      done();
    });
  });
});
