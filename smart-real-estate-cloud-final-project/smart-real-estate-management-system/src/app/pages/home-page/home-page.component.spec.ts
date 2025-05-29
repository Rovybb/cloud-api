import { TestBed, ComponentFixture } from '@angular/core/testing';
import { HomePageComponent } from './home-page.component';
import { Router } from '@angular/router';
import { LoginService } from '../../services/identity/login.service';
import { NavbarComponent } from '../../components/navbar/navbar.component';
import { NavbarHomeComponent } from '../../components/navbar-home/navbar-home.component';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatButtonModule } from '@angular/material/button';
import { of } from 'rxjs';
import { CommonModule } from '@angular/common';

describe('HomePageComponent', () => {
  let component: HomePageComponent;
  let fixture: ComponentFixture<HomePageComponent>;
  let routerSpy: jasmine.SpyObj<Router>;
  let loginServiceSpy: jasmine.SpyObj<LoginService>;

  beforeEach(async () => {
    // Creăm spy-uri pentru Router și LoginService
    routerSpy = jasmine.createSpyObj('Router', ['navigate']);
    loginServiceSpy = jasmine.createSpyObj('LoginService', ['isAuthenticated', 'logout']);

    await TestBed.configureTestingModule({
      imports: [
        CommonModule,
        MatSidenavModule,
        MatToolbarModule,
        MatButtonModule,
        NavbarComponent,
        NavbarHomeComponent,
        HomePageComponent  // Importăm componenta standalone aici
      ],
      providers: [
        { provide: Router, useValue: routerSpy },
        { provide: LoginService, useValue: loginServiceSpy }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(HomePageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create the component', () => {
    expect(component).toBeTruthy();
  });

  it('should check if the user is logged in', () => {
    loginServiceSpy.isAuthenticated.and.returnValue(of(true));
    component.isLogged().subscribe((isLogged) => {
      expect(isLogged).toBeTrue();
    });
    expect(loginServiceSpy.isAuthenticated).toHaveBeenCalled();
  });

  it('should navigate to properties when navigateToProperties is called', () => {
    component.navigateToProperties();
    expect(routerSpy.navigate).toHaveBeenCalledWith(['properties']);
  });

  it('should navigate to register when navigateToRegister is called', () => {
    component.navigateToRegister();
    expect(routerSpy.navigate).toHaveBeenCalledWith(['auth/register']);
  });

  it('should navigate to login when navigateToLogin is called', () => {
    component.navigateToLogin();
    expect(routerSpy.navigate).toHaveBeenCalledWith(['auth/login']);
  });

  it('should call logout on the LoginService when logout is called', () => {
    component.logout();
    expect(loginServiceSpy.logout).toHaveBeenCalled();
  });
});
