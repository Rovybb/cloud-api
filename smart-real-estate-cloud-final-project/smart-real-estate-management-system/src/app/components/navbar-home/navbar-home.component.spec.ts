import { ComponentFixture, TestBed } from '@angular/core/testing';
import { Router } from '@angular/router';
import { NavbarHomeComponent } from './navbar-home.component';
import { LoginService } from '../../services/identity/login.service';

describe('NavbarHomeComponent', () => {
  let component: NavbarHomeComponent;
  let fixture: ComponentFixture<NavbarHomeComponent>;
  let loginServiceSpy: jasmine.SpyObj<LoginService>;
  let routerSpy: jasmine.SpyObj<Router>;

  beforeEach(async () => {
    // Creăm spy-uri pentru loginService și router
    const loginServiceMock = jasmine.createSpyObj('LoginService', ['logout']);
    const routerMock = jasmine.createSpyObj('Router', ['navigate']);

    await TestBed.configureTestingModule({
      // Deoarece NavbarHomeComponent este standalone, îl importăm aici
      imports: [NavbarHomeComponent],
      providers: [
        { provide: LoginService, useValue: loginServiceMock },
        { provide: Router, useValue: routerMock }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(NavbarHomeComponent);
    component = fixture.componentInstance;
    loginServiceSpy = TestBed.inject(LoginService) as jasmine.SpyObj<LoginService>;
    routerSpy = TestBed.inject(Router) as jasmine.SpyObj<Router>;
    fixture.detectChanges(); // Inițializează componenta
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should call loginService.logout when logout is called', () => {
    // Apelăm metoda logout din componentă.
    component.logout();
    // Verificăm că metoda logout din LoginService a fost apelată.
    expect(loginServiceSpy.logout).toHaveBeenCalled();
  });
});
