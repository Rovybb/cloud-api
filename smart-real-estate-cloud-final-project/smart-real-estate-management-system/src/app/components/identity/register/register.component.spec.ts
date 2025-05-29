import { ComponentFixture, TestBed, fakeAsync, tick } from '@angular/core/testing';
import { ReactiveFormsModule } from '@angular/forms';
import { of, throwError } from 'rxjs';

import { RegisterComponent } from './register.component';
import { RegisterService } from '../../../services/identity/register.service';

describe('RegisterComponent', () => {
  let component: RegisterComponent;
  let fixture: ComponentFixture<RegisterComponent>;
  let registerServiceSpy: jasmine.SpyObj<RegisterService>;

  beforeEach(async () => {
    // Creăm un spy pentru RegisterService
    const spy = jasmine.createSpyObj('RegisterService', ['register']);

    await TestBed.configureTestingModule({
      // Deoarece RegisterComponent este standalone, îl importăm în array-ul imports.
      // De asemenea, importăm ReactiveFormsModule pentru a lucra cu formularele reactive.
      imports: [RegisterComponent, ReactiveFormsModule],
      providers: [{ provide: RegisterService, useValue: spy }]
    }).compileComponents();

    fixture = TestBed.createComponent(RegisterComponent);
    component = fixture.componentInstance;
    registerServiceSpy = TestBed.inject(RegisterService) as jasmine.SpyObj<RegisterService>;
    fixture.detectChanges();
  });

  it('should create the component', () => {
    expect(component).toBeTruthy();
  });

  it('should return validation error if password and confirmPassword do not match', () => {
    // Setăm valori: email valid, dar password și confirmPassword diferite.
    component.registerForm.controls['email'].setValue('test@example.com');
    component.registerForm.controls['password'].setValue('12345678');
    component.registerForm.controls['confirmPassword'].setValue('87654321');

    const validationResult = component.passwordMatchValidator(component.registerForm);
    expect(validationResult).toEqual({ mustMatch: true });
  });

  it('should submit the form successfully when valid', () => {
    const validEmail = 'test@example.com';
    const validPassword = '12345678';

    // Setăm valori valide pentru toate câmpurile
    component.registerForm.controls['email'].setValue(validEmail);
    component.registerForm.controls['password'].setValue(validPassword);
    component.registerForm.controls['confirmPassword'].setValue(validPassword);

    // Simulăm un răspuns de succes din registerService
    registerServiceSpy.register.and.returnValue(of({}));

    component.onSubmit();

    expect(registerServiceSpy.register).toHaveBeenCalledWith(validEmail, validPassword);
    expect(component.successMessage).toBe('Registration successful!');
    // Formularul trebuie resetat după o înregistrare reușită.
    expect(component.registerForm.pristine).toBeTrue();
  });

  it('should show error message when registerService returns an error', () => {
    const validEmail = 'test@example.com';
    const validPassword = '12345678';

    // Setăm valori valide pentru câmpuri
    component.registerForm.controls['email'].setValue(validEmail);
    component.registerForm.controls['password'].setValue(validPassword);
    component.registerForm.controls['confirmPassword'].setValue(validPassword);

    const errorResponse = { error: { message: 'User already exists' } };
    registerServiceSpy.register.and.returnValue(throwError(() => errorResponse));

    component.onSubmit();

    expect(registerServiceSpy.register).toHaveBeenCalledWith(validEmail, validPassword);
    expect(component.errorMessage).toBe('User already exists');
  });

  it('should show a validation error message when the form is invalid', () => {
    // Setăm valori invalide pentru a face formularul invalid
    component.registerForm.controls['email'].setValue('invalid-email');
    component.registerForm.controls['password'].setValue('');
    component.registerForm.controls['confirmPassword'].setValue('');

    component.onSubmit();

    expect(component.errorMessage).toBe('Please fix validation errors before submitting.');
    expect(registerServiceSpy.register).not.toHaveBeenCalled();
  });

  it('should clear success and error messages after 10 seconds', fakeAsync(() => {
    // Setăm manual mesaje de succes și eroare
    component.errorMessage = 'Test error message';
    component.successMessage = 'Test success message';

    // Apelăm metoda ce pornește timerul de clear
    // Pentru a accesa metoda privată, se poate utiliza component['clearMessagesAfterTimeout']()
    component['clearMessagesAfterTimeout']();

    // Înainte de trecerea timpului, mesajele există
    expect(component.errorMessage).toBe('Test error message');
    expect(component.successMessage).toBe('Test success message');

    // Simulăm trecerea a 10 secunde (10000 ms)
    tick(10000);

    // După expirarea timerului, mesajele trebuie să fie șterse
    expect(component.errorMessage).toBeNull();
    expect(component.successMessage).toBeNull();
  }));
});
