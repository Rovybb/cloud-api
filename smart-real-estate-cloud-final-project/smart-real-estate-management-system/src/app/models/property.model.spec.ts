import { Property, PropertyType, PropertyStatus } from './property.model';

describe('Property Model', () => {
  describe('PropertyType Enum', () => {
    it('should contain valid property types', () => {
      expect(PropertyType.HOUSE).toBe('HOUSE');
      expect(PropertyType.APARTMENT).toBe('APARTMENT');
      expect(PropertyType.LAND).toBe('LAND');
      expect(PropertyType.COMMERCIAL).toBe('COMMERCIAL');
    });
  });

  describe('PropertyStatus Enum', () => {
    it('should contain valid property statuses', () => {
      expect(PropertyStatus.AVAILABLE).toBe('AVAILABLE');
      expect(PropertyStatus.SOLD).toBe('SOLD');
      expect(PropertyStatus.RENTED).toBe('RENTED');
    });


  });

  describe('Property Interface', () => {
    it('should accept valid properties', () => {
      const validProperty: Property = {
        id: '123',
        title: 'Beautiful House',
        description: 'A lovely house with 3 bedrooms.',
        type: PropertyType.HOUSE,
        status: PropertyStatus.AVAILABLE,
        price: 300000,
        address: '123 Main St',
        area: 120,
        rooms: 3,
        bathrooms: 2,
        constructionYear: 2005,
        createdAt: new Date('2023-01-01'),
        updatedAt: new Date('2023-01-02'),
        userId: 'user123',
        imageUrls: ['https://example.com/image1.jpg'],
      };

      expect(validProperty.title).toBe('Beautiful House');
      expect(validProperty.type).toBe(PropertyType.HOUSE);
    });

    it('should throw an error for invalid properties (type mismatch)', () => {
      // Simulăm eroare folosind cast greșit
      const invalidProperty: any = {
        id: '123',
        title: 'Invalid Property',
        description: 'A property with incorrect type.',
        type: 'INVALID_TYPE', // Invalid value
        status: PropertyStatus.AVAILABLE,
        price: -100000, // Negative price
        address: '123 Main St',
        area: 120,
        rooms: 3,
        bathrooms: 2,
        constructionYear: 2005,
        createdAt: new Date('2023-01-01'),
        updatedAt: new Date('2023-01-02'),
        userId: 'user123',
      };

      expect(() => {
        const castedProperty: Property = invalidProperty;
      }).toThrowError();
    });
  });
});
