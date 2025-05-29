// property.service.spec.ts

import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { PropertyService } from './property.service';
import { PropertyStatus } from '../models/property.model';
import { environment } from '../../environments/environment';
import { Property, PropertyType } from '../models/property.model';

describe('PropertyService', () => {
  let service: PropertyService;
  let httpMock: HttpTestingController;
  const testToken = 'my-test-token';

  const dummyProperties: Property[] = [
    {
      id: '1',
      title: 'Test Property 1',
      description: 'Description 1',
      status: PropertyStatus.AVAILABLE,
      type: PropertyType.HOUSE,
      price: 100000,
      address: 'Str. Exemplu 123',
      area: 120,
      rooms: 3,
      bathrooms: 2,
      constructionYear: 2000,
      createdAt: new Date(),
      updatedAt: new Date(),
      userId: ''
    },
    {
      id: '2',
      title: 'Test Property 2',
      description: 'Description 2',
      status: PropertyStatus.SOLD,
      type: PropertyType.APARTMENT,
      price: 80000,
      address: 'Av. Exemplu 456',
      area: 90,
      rooms: 2,
      bathrooms: 1,
      constructionYear: 2010,
      createdAt: new Date(),
      updatedAt: new Date(),
      userId: ''
    }
  ];

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [PropertyService]
    });
    service = TestBed.inject(PropertyService);
    httpMock = TestBed.inject(HttpTestingController);

    // Setăm cookie-ul cu token pentru a fi folosit de getTokenFromCookies()
    document.cookie = `token=${testToken}`;
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should retrieve properties with GET', () => {
    service.getProperties().subscribe((properties) => {
      expect(properties.length).toBe(2);
      expect(properties).toEqual(dummyProperties);
    });

    const req = httpMock.expectOne(`${environment.API_URL}/Properties`);
    expect(req.request.method).toBe('GET');
    // Pentru getProperties nu se setează headerul Authorization în codul dat
    req.flush(dummyProperties);
  });

  it('should send inquiry with POST and proper JWT header', () => {
    const inquiryPayload = {
      propertyId: '1',
      message: 'Vreau mai multe detalii',
      status: 1,
      agentId: 'agent-123',
      clientId: 'client-456'
    };

    service.sendInquiry(
      inquiryPayload.propertyId,
      inquiryPayload.message,
      inquiryPayload.status,
      inquiryPayload.agentId,
      inquiryPayload.clientId
    ).subscribe((response) => {
      expect(response).toEqual({ success: true });
    });

    const req = httpMock.expectOne('https://localhost:7146/api/v1/Inquiries');
    expect(req.request.method).toBe('POST');
    expect(req.request.headers.get('Authorization')).toBe(`Bearer ${testToken}`);
    expect(req.request.body).toEqual(inquiryPayload);
    req.flush({ success: true });
  });

  it('should retrieve properties with pagination and filters with GET and JWT header', () => {
    const pageNumber = 1;
    const pageSize = 10;
    const filters: { status: string; type: string } = { status: 'available', type: 'House' };

    service.getPropertiesWithPagination(pageNumber, pageSize, filters).subscribe((response) => {
      expect(response).toEqual({ data: dummyProperties });
    });

    // Construim parametrii așa cum este folosit în cod
    let params = `?pageNumber=${pageNumber}&pageSize=${pageSize}`;
    for (const key in filters) {
      params += `&${key}=${filters[key as keyof typeof filters]}`;
    }

    const req = httpMock.expectOne(
      (request) => request.url === `${environment.API_URL}/Properties`
        && request.params.get('pageNumber') === pageNumber.toString()
        && request.params.get('pageSize') === pageSize.toString()
        && request.params.get('status') === filters.status
        && request.params.get('type') === filters.type
    );
    expect(req.request.method).toBe('GET');
    expect(req.request.headers.get('Authorization')).toBe(`Bearer ${testToken}`);
    req.flush({ data: dummyProperties });
  });

  it('should create property with POST and JWT header', () => {
    const newProperty: Property = {
      id: '3',
      title: 'New Property',
      description: 'New description',
      status: PropertyStatus.AVAILABLE,
      type: PropertyType.APARTMENT,
      price: 150000,
      address: 'Str. Noua 789',
      area: 200,
      rooms: 5,
      bathrooms: 3,
      constructionYear: 2020,
      createdAt: new Date(),
      updatedAt: new Date(),
      userId: ''
    };

    service.createProperty(newProperty).subscribe((response) => {
      expect(response).toEqual(newProperty);
    });

    const req = httpMock.expectOne(`${environment.API_URL}/Properties`);
    expect(req.request.method).toBe('POST');
    expect(req.request.headers.get('Authorization')).toBe(`Bearer ${testToken}`);
    expect(req.request.body).toEqual(newProperty);
    req.flush(newProperty);
  });

  it('should update property with PUT and JWT header', () => {
    const updatedProperty: Property = {
      id: '1',
      title: 'Updated Property',
      description: 'Updated description',
      status: PropertyStatus.AVAILABLE,
      type: PropertyType.HOUSE,
      price: 110000,
      address: 'Str. Actualizată 101',
      area: 130,
      rooms: 4,
      bathrooms: 2,
      constructionYear: 2005,
      createdAt: new Date(),
      updatedAt: new Date(),
      userId: ''
    };

    service.updateProperty('1', updatedProperty).subscribe((response) => {
      expect(response).toEqual(updatedProperty);
    });

    const req = httpMock.expectOne(`${environment.API_URL}/Properties/1`);
    expect(req.request.method).toBe('PUT');
    expect(req.request.headers.get('Authorization')).toBe(`Bearer ${testToken}`);
    expect(req.request.body).toEqual(updatedProperty);
    req.flush(updatedProperty);
  });

  it('should delete property with DELETE and JWT header', () => {
    const propertyId = '1';
    service.deleteProperty(propertyId).subscribe((response) => {
      expect(response).toEqual({ success: true });
    });

    const req = httpMock.expectOne(`${environment.API_URL}/Properties/${propertyId}`);
    expect(req.request.method).toBe('DELETE');
    expect(req.request.headers.get('Authorization')).toBe(`Bearer ${testToken}`);
    req.flush({ success: true });
  });

  it('should retrieve property by id with GET and JWT header', () => {
    const propertyId = '1';
    service.getPropertyById(propertyId).subscribe((response) => {
      expect(response).toEqual(dummyProperties[0]);
    });

    const req = httpMock.expectOne(`${environment.API_URL}/Properties/${propertyId}`);
    expect(req.request.method).toBe('GET');
    expect(req.request.headers.get('Authorization')).toBe(`Bearer ${testToken}`);
    req.flush(dummyProperties[0]);
  });

  it('should get property images with GET and JWT header', () => {
    const propertyId = '1';
    const dummyImages = [{ id: 'img1', url: 'http://example.com/img1.jpg' }];
    
    service.getPropertyImages(propertyId).subscribe((images) => {
      expect(images).toEqual(dummyImages);
    });

    const req = httpMock.expectOne(`${environment.API_URL}/Properties/${propertyId}/images`);
    expect(req.request.method).toBe('GET');
    expect(req.request.headers.get('Authorization')).toBe(`Bearer ${testToken}`);
    req.flush(dummyImages);
  });

  it('should upload image with POST and JWT header', () => {
    const propertyId = '1';
    const formData = new FormData();
    formData.append('file', new Blob(['image content'], { type: 'image/jpeg' }), 'test.jpg');
    const uploadResponse = { success: true };

    service.uploadImage(propertyId, formData).subscribe((response) => {
      expect(response).toEqual(uploadResponse);
    });

    const req = httpMock.expectOne(`${environment.API_URL}/Properties/${propertyId}/images`);
    expect(req.request.method).toBe('POST');
    expect(req.request.headers.get('Authorization')).toBe(`Bearer ${testToken}`);
    req.flush(uploadResponse);
  });

  it('should create checkout session with POST and JWT header', () => {
    const checkoutBody = { amount: 500, currency: 'USD' };
    const checkoutResponse = { sessionId: 'sess_123' };

    service.createCheckoutSession(checkoutBody).subscribe((response) => {
      expect(response).toEqual(checkoutResponse);
    });

    const req = httpMock.expectOne('https://localhost:7146/api/v1/Payments/create-checkout-session');
    expect(req.request.method).toBe('POST');
    expect(req.request.headers.get('Authorization')).toBe(`Bearer ${testToken}`);
    expect(req.request.body).toEqual(checkoutBody);
    req.flush(checkoutResponse);
  });

  it('should predict price with POST and JWT header', () => {
    const predictionBody = { area: 150, rooms: 4 };
    const predictionResponse = { predictedPrice: 200000 };

    service.predictPrice(predictionBody).subscribe((response) => {
      expect(response).toEqual(predictionResponse);
    });

    const req = httpMock.expectOne('https://localhost:7146/api/v1/PropertyPricePrediction/predict');
    expect(req.request.method).toBe('POST');
    expect(req.request.headers.get('Authorization')).toBe(`Bearer ${testToken}`);
    expect(req.request.body).toEqual(predictionBody);
    req.flush(predictionResponse);
  });

});
