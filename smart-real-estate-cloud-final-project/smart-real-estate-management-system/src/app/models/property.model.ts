export enum PropertyType {
    HOUSE = "HOUSE",
    APARTMENT = "APARTMENT",
    LAND = "LAND",
    COMMERCIAL = "COMMERCIAL"
  }
  
  export enum PropertyStatus {
    AVAILABLE = "AVAILABLE",
    SOLD = "SOLD",
    RENTED = "RENTED"
  }

  // TO-DO: Adaptează câmpurile la structura din .NET + de spart in mai multe fisiere

export interface Property {
    id: string; 
    title: string;
    description: string;
    type: PropertyType;
    status: PropertyStatus;
    price: number;
    address: string;
    area: number;
    rooms: number;
    bathrooms: number;
    constructionYear: number;
    createdAt: Date; 
    updatedAt: Date; 
    userId: string;
    imageUrls?: string[]; 
  }