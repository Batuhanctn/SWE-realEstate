export interface Property {
  Id?: string;
  title: string;
  description: string;
  price: number;
  address: string;
  location: string;
  city: string;
  state: string;
  zipCode: string;
  type: string;
  propertyType: string;
  status: string;
  imageUrl: string;
  imageUrls: string[];
  images: string[];
  bedrooms: number;
  bathrooms: number;
  squareMeters: number;
  size: number;
  isForRent: boolean;
  features: string[];
  agent?: {
    id: string;
    name: string;
    email: string;
    phone: string;
    photo?: string;
  };
  createdAt?: string;
  updatedAt?: string;
  userId?: string;
}
