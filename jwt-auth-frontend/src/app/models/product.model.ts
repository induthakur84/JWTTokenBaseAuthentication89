// DTOs matching backend ProductController

export interface Product {
  id: number;
  name: string;
  description: string;
}

export interface ProductCreate {
  name: string;
  description: string;
}

export interface ProductUpdate {
  id: number;
  name: string;
  description: string;
}
