import { ComponentFixture, TestBed } from '@angular/core/testing';
import { PropertyCreateComponent } from './property-create.component';
import { FormBuilder, ReactiveFormsModule } from '@angular/forms';
import { PropertyService } from '../../services/property.service';
import { Router } from '@angular/router';
import { of, throwError } from 'rxjs';
import { HttpClientTestingModule } from '@angular/common/http/testing';

// Înlocuim jwtDecode cu o funcție simplificată
function mockJwtDecode(): { unique_name: string } {
  return {
    unique_name: '6fa459ea-ee8a-3ca4-894e-db77e160355e'
  };
}

describe('PropertyCreateComponent', () => {
  let component: PropertyCreateComponent;
  let fixture: ComponentFixture<PropertyCreateComponent>;
  let propertyServiceMock: jasmine.SpyObj<PropertyService>;
  let routerMock: jasmine.SpyObj<Router>;

  beforeEach(async () => {
    propertyServiceMock = jasmine.createSpyObj('PropertyService', ['createProperty']);
    routerMock = jasmine.createSpyObj('Router', ['navigate']);

    await TestBed.configureTestingModule({
      imports: [
        PropertyCreateComponent,
        ReactiveFormsModule,
        HttpClientTestingModule // Importăm pentru HttpClient
      ],
      providers: [
        FormBuilder,
        { provide: PropertyService, useValue: propertyServiceMock },
        { provide: Router, useValue: routerMock },
        // Înlocuim jwtDecode cu mock-ul nostru
        { provide: 'jwtDecode', useValue: mockJwtDecode }
      ],
    }).compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(PropertyCreateComponent);
    component = fixture.componentInstance;

    // Mock pentru metoda getTokenFromCookie
    spyOn(component as any, 'getTokenFromCookie').and.returnValue('FAKE_TOKEN');

    fixture.detectChanges(); // Rulează ngOnInit
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should initialize the form with default values and add userId from token', () => {
    const form = component.propertyForm;
    expect(form).toBeDefined();
    expect(form.value.userId).toBe('6fa459ea-ee8a-3ca4-894e-db77e160355e');
  });

  it('should validate the form fields correctly', () => {
    const form = component.propertyForm;
    form.controls['title'].setValue('');
    form.controls['description'].setValue('');
    form.controls['price'].setValue(-1);

    expect(form.valid).toBeFalse();
    expect(form.controls['title'].valid).toBeFalse();
    expect(form.controls['description'].valid).toBeFalse();
    expect(form.controls['price'].valid).toBeFalse();
  });

  it('should submit the form if valid', () => {
    const form = component.propertyForm;
    form.setValue({
      title: 'Test Property',
      description: 'This is a test property.',
      status: 'AVAILABLE',
      type: 'APARTMENT',
      price: 100000,
      address: '123 Test St',
      area: 150,
      rooms: 4,
      bathrooms: 2,
      constructionYear: 2020,
      userId: '6fa459ea-ee8a-3ca4-894e-db77e160355e',
    });

    propertyServiceMock.createProperty.and.returnValue(of({}));

    component.onSubmit();

    expect(form.valid).toBeTrue();
    expect(propertyServiceMock.createProperty).toHaveBeenCalledWith(form.value);
    expect(routerMock.navigate).toHaveBeenCalledWith(['/properties']);
  });

  it('should handle errors during submission', () => {
    const form = component.propertyForm;
    form.setValue({
      title: 'Test Property',
      description: 'This is a test property.',
      status: 'AVAILABLE',
      type: 'APARTMENT',
      price: 100000,
      address: '123 Test St',
      area: 150,
      rooms: 4,
      bathrooms: 2,
      constructionYear: 2020,
      userId: '6fa459ea-ee8a-3ca4-894e-db77e160355e',
    });

    propertyServiceMock.createProperty.and.returnValue(
      throwError(() => new Error('Create error'))
    );

    component.onSubmit();
 
    expect(propertyServiceMock.createProperty).toHaveBeenCalledWith(form.value);
    expect(component.errorMessage).toBe('Error creating property. Please try again.');
    expect(routerMock.navigate).not.toHaveBeenCalled();
  });
});   