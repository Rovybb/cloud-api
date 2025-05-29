import { ComponentFixture, TestBed } from '@angular/core/testing';
import { of } from 'rxjs';
import { PropertyListComponent } from './property-list.component';
import { PropertyService } from '../../services/property.service';
import { Router } from '@angular/router';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { PropertyStatus, PropertyType } from '../../models/property.model';

export interface Property {
  id: string;
  title: string;
  description: string;
  status: PropertyStatus;
  type: PropertyType;
  price: number;
  address: string;
  area: number;
  rooms: number;
  bathrooms: number;
  constructionYear: number;
  userId: string;
  createdAt: Date;
  updatedAt: Date;
}

describe('PropertyListComponent', () => {
  let component: PropertyListComponent;
  let fixture: ComponentFixture<PropertyListComponent>;
  let propertyServiceSpy: jasmine.SpyObj<PropertyService>;
  let routerSpy: jasmine.SpyObj<Router>;

  beforeEach(async () => {
    const propertyServiceMock = jasmine.createSpyObj('PropertyService', ['getPropertiesWithPagination']);
    const routerMock = jasmine.createSpyObj('Router', ['navigate']);

    propertyServiceMock.getPropertiesWithPagination.and.returnValue(of({ items: [], totalPages: 0 }));

    await TestBed.configureTestingModule({
      imports: [PropertyListComponent, HttpClientTestingModule],
      providers: [
        { provide: PropertyService, useValue: propertyServiceMock },
        { provide: Router, useValue: routerMock }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(PropertyListComponent);
    component = fixture.componentInstance;
    propertyServiceSpy = TestBed.inject(PropertyService) as jasmine.SpyObj<PropertyService>;
    routerSpy = TestBed.inject(Router) as jasmine.SpyObj<Router>;

    fixture.detectChanges();
  });

  it('should create the component', () => {
    expect(component).toBeTruthy();
  });

  describe('loadProperties', () => {
    it('should call getPropertiesWithPagination with processed filters and update component data', () => {
      const mockResponse = {
        items: [
          { id: '1', title: 'Test Property', price: 100 }
        ],
        totalPages: 3
      };

      propertyServiceSpy.getPropertiesWithPagination.and.returnValue(of(mockResponse));

      component.filters = {
        title: 'House',
        price_min: null,
        price_max: '',
        description: 'Nice property'
      };

      component.loadProperties();

      expect(propertyServiceSpy.getPropertiesWithPagination).toHaveBeenCalledWith(
        component.pageNumber,
        component.pageSize,
        { title: 'House', description: 'Nice property' }
      );

      expect(component.properties.length).toBe(1);
      expect(component.totalPages).toBe(3);
    });
  });

  describe('navigation methods', () => {
    it('should navigate to create property page when navigateToCreate is called', () => {
      component.navigateToCreate();
      expect(routerSpy.navigate).toHaveBeenCalledWith(['properties/create']);
    });

    it('should navigate to property details when viewDetails is called', () => {
      const testProperty: Property = {
        id: '6fa459ea-ee8a-3ca4-894e-db77e160355e',
        title: 'Test',
        description: 'Test description',
        status: PropertyStatus.AVAILABLE,
        type: PropertyType.APARTMENT,
        price: 999,
        address: '123 Test St',
        area: 100,
        rooms: 3,
        bathrooms: 2,
        constructionYear: 2020,
        userId: '6fa459ea-ee8a-3ca4-894e-db77e160355e',
        createdAt: new Date(),
        updatedAt: new Date()
      };

      expect(routerSpy.navigate).toHaveBeenCalledWith(['/properties/property-details', testProperty.id]);
    });
  });

  describe('filters', () => {
    beforeEach(() => {
      spyOn(component, 'loadProperties');
    });

    it('applyFilters should set pageNumber to 1 and call loadProperties', () => {
      component.pageNumber = 5;
      component.applyFilters();
      expect(component.pageNumber).toBe(1);
      expect(component.loadProperties).toHaveBeenCalled();
    });

    it('resetFilters should clear filters, set pageNumber to 1, and call loadProperties', () => {
      component.filters = {
        title: 'Something',
        price_min: 50,
        price_max: 100,
        description: 'Test desc'
      };
      component.pageNumber = 3;

      component.resetFilters();

      expect(component.filters).toEqual({
        title: null,
        price_min: null,
        price_max: null,
        description: null
      });
      expect(component.pageNumber).toBe(1);
      expect(component.loadProperties).toHaveBeenCalled();
    });
  });

  describe('pagination methods', () => {
    beforeEach(() => {
      spyOn(component, 'loadProperties');
      component.totalPages = 5;
    });

    it('goToPage should change pageNumber and call loadProperties if page is valid', () => {
      component.goToPage(3);
      expect(component.pageNumber).toBe(3);
      expect(component.loadProperties).toHaveBeenCalled();
    });

    it('goToPage should not call loadProperties if page is outside the range', () => {
      component.pageNumber = 2;
      component.goToPage(10);
      expect(component.pageNumber).toBe(2);
      expect(component.loadProperties).not.toHaveBeenCalled();
    });

    it('previousPage should decrement pageNumber and call loadProperties if pageNumber > 1', () => {
      component.pageNumber = 3;
      component.previousPage();
      expect(component.pageNumber).toBe(2);
      expect(component.loadProperties).toHaveBeenCalled();
    });

    it('previousPage should not decrement pageNumber if pageNumber is already 1', () => {
      component.pageNumber = 1;
      component.previousPage();
      expect(component.pageNumber).toBe(1);
      expect(component.loadProperties).not.toHaveBeenCalled();
    });

    it('nextPage should increment pageNumber and call loadProperties if pageNumber < totalPages', () => {
      component.pageNumber = 2;
      component.nextPage();
      expect(component.pageNumber).toBe(3);
      expect(component.loadProperties).toHaveBeenCalled();
    });

    it('nextPage should not increment pageNumber if pageNumber equals totalPages', () => {
      component.pageNumber = 5;
      component.nextPage();
      expect(component.pageNumber).toBe(5);
      expect(component.loadProperties).not.toHaveBeenCalled();
    });
  });
});