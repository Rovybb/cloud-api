import { ComponentFixture, TestBed, fakeAsync, tick } from '@angular/core/testing';
import { PropertyDetailComponent } from './property-detail.component';
import { PropertyService } from '../../services/property.service';
import { Router } from '@angular/router';
import { ActivatedRoute } from '@angular/router';
import { of, throwError } from 'rxjs';
import { CommonModule } from '@angular/common';
import { HttpClientTestingModule } from '@angular/common/http/testing';

describe('PropertyDetailComponent', () => {
  let component: PropertyDetailComponent;
  let fixture: ComponentFixture<PropertyDetailComponent>;
  let propertyServiceMock: any;
  let routerMock: any;
  let activatedRouteMock: any;

  beforeEach(async () => {
    propertyServiceMock = {
      getPropertyById: jasmine.createSpy('getPropertyById'),
      deleteProperty: jasmine.createSpy('deleteProperty')
    };

    routerMock = jasmine.createSpyObj('Router', ['navigate']);

    activatedRouteMock = {
      snapshot: {
        paramMap: {
          get: jasmine.createSpy('get').and.returnValue('1')
        }
      }
    };

    await TestBed.configureTestingModule({
      imports: [PropertyDetailComponent, CommonModule, HttpClientTestingModule],
      providers: [
        { provide: PropertyService, useValue: propertyServiceMock },
        { provide: Router, useValue: routerMock },
        { provide: ActivatedRoute, useValue: activatedRouteMock }
      ]
    }).compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(PropertyDetailComponent);
    component = fixture.componentInstance;
    // Pentru testele care nu se referă la eroare, setăm ca spy să returneze date valide:
    propertyServiceMock.getPropertyById.and.returnValue(of({
      id: '8c868c11-e8db-4d11-a7c8-83ccb221305a',
      title: 'Modern Apartment',
      description: 'A beautiful apartment in the city center.',
      type: 0,
      status: 0,
      price: 100000,
      address: '123 Main St',
      area: 120,
      rooms: 3,
      bathrooms: 2,
      constructionYear: 2015,
      createdAt: new Date(),
      updatedAt: new Date(),
      userId: '3c868c18-e8db-4d11-a7c8-83ccb221305a'
    }));
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should load property details on initialization', () => {
    expect(propertyServiceMock.getPropertyById).toHaveBeenCalledWith('1');
    expect(component.property).toBeTruthy();
    expect(component.property?.title).toBe('Modern Apartment');
  });

  it('should handle error when property details are not found', fakeAsync(() => {
    // Configurăm spy-ul să returneze un Observable ce emite eroarea
    propertyServiceMock.getPropertyById.and.returnValue(throwError(() => new Error('Property not found')));
    
    // Refacem inițializarea componentei pentru a declanșa ngOnInit care apelează loadProperties
    component.ngOnInit();
    tick(); // Așteptăm rezolvarea microtask-urilor

    expect(component.errorMessage).toBe('Error fetching property details. Please try again.');
    expect(routerMock.navigate).toHaveBeenCalledWith(['/properties']);
  }));

  it('should navigate to update property page', () => {
    const property = component.property!;
    component.navigateToUpdate(property);
    expect(routerMock.navigate).toHaveBeenCalledWith(['properties/update/' + property.id]);
  });
  
  it('should confirm before deleting property and show error if deletion fails', () => {
    const property = component.property!;
    spyOn(window, 'confirm').and.returnValue(true);
    propertyServiceMock.deleteProperty.and.returnValue(throwError(() => new Error('Delete error')));
    spyOn(window, 'alert');

    component.deleteProperty(property);

    expect(propertyServiceMock.deleteProperty).toHaveBeenCalledWith(property.id);
    expect(window.alert).toHaveBeenCalledWith('Failed to delete property.');
  });

  it('should navigate back to properties list', () => {
    component.goBack();
    expect(routerMock.navigate).toHaveBeenCalledWith(['/properties']);
  });

  it('should display correct property status', () => {
    const status = component.getPropertyStatus(0);
    expect(status).toBe('AVAILABLE');
  });

  it('should display correct property type', () => {
    const type = component.getPropertyType(0); 
    expect(type).toBe('HOUSE');
  });

  it('should handle invalid status and type', () => {
    const invalidStatus = component.getPropertyStatus(99);
    const invalidType = component.getPropertyType(99);
    expect(invalidStatus).toBe('UNKNOWN');
    expect(invalidType).toBe('UNKNOWN');
  });
});
