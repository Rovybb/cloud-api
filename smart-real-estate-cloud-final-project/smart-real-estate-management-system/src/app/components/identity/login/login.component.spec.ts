import { ComponentFixture, TestBed } from '@angular/core/testing';
import { LoginComponent } from './login.component';
import { FormBuilder, ReactiveFormsModule } from '@angular/forms';
import { LoginService } from '../../../services/identity/login.service';
import { of, throwError } from 'rxjs';
import { CommonModule } from '@angular/common';

describe('LoginComponent', () => {
  let component: LoginComponent;
  let fixture: ComponentFixture<LoginComponent>;
  let loginServiceMock: any;

  beforeEach(async () => {
    loginServiceMock = {
      login: jasmine.createSpy('login'),
      setAuthenticationState: jasmine.createSpy('setAuthenticationState')
    };

    await TestBed.configureTestingModule({
      imports: [LoginComponent, ReactiveFormsModule, CommonModule],
      providers: [
        FormBuilder,
        { provide: LoginService, useValue: loginServiceMock }
      ]
    }).compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(LoginComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create the LoginComponent', () => {
    expect(component).toBeTruthy();
  });

  it('should initialize the form with default values', () => {
    const form = component.loginForm;
    expect(form).toBeDefined();
    expect(form.controls['email'].value).toBe('');
    expect(form.controls['password'].value).toBe('');
  });

  it('should validate the form fields correctly', () => {
    const form = component.loginForm;

    // Initially empty
    form.controls['email'].setValue('');
    form.controls['password'].setValue('');
    expect(form.valid).toBeFalse();

    // Invalid email
    form.controls['email'].setValue('invalidemail');
    form.controls['password'].setValue('short');
    expect(form.controls['email'].valid).toBeFalse();
    expect(form.controls['password'].valid).toBeFalse();
    expect(form.valid).toBeFalse();

    // Valid fields
    form.controls['email'].setValue('test@example.com');
    form.controls['password'].setValue('password123');
    expect(form.controls['email'].valid).toBeTrue();
    expect(form.controls['password'].valid).toBeTrue();
    expect(form.valid).toBeTrue();
  });

  it('should submit the form if valid', () => {
    const form = component.loginForm;
    form.setValue({
      email: 'test@example.com',
      password: 'password123'
    });

    loginServiceMock.login.and.returnValue(of({}));

    component.onSubmit();

    expect(form.valid).toBeTrue();
    expect(loginServiceMock.login).toHaveBeenCalledWith('test@example.com', 'password123');
    expect(loginServiceMock.setAuthenticationState).toHaveBeenCalledWith(true);
    expect(component.errorMessage).toBeNull();
  });

  it('should not submit the form if invalid', () => {
    const form = component.loginForm;
    form.controls['email'].setValue('');
    form.controls['password'].setValue('');

    component.onSubmit();

    expect(form.valid).toBeFalse();
    expect(loginServiceMock.login).not.toHaveBeenCalled();
    expect(loginServiceMock.setAuthenticationState).not.toHaveBeenCalled();
    expect(component.errorMessage).toBe('Please fix validation errors before submitting.');
  });

  it('should handle login errors', () => {
    const form = component.loginForm;
    form.setValue({
      email: 'test@example.com',
      password: 'password123'
    });

    loginServiceMock.login.and.returnValue(throwError(() => new Error('Login error')));

    component.onSubmit();

    expect(loginServiceMock.login).toHaveBeenCalledWith('test@example.com', 'password123');
    expect(component.errorMessage).toBe('Login failed. Please try again.');
    expect(loginServiceMock.setAuthenticationState).not.toHaveBeenCalled();
  });
});
