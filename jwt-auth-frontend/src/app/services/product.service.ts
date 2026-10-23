import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Product, ProductCreate, ProductUpdate } from '../models/product.model';
import { AuthService } from './auth.service';

@Injectable({
  providedIn: 'root'
})
export class ProductService {
  private http = inject(HttpClient);
  private authService = inject(AuthService);

  private get apiUrl(): string {
    return this.authService.apiUrl();
  }

  /**
   * GET ALL PRODUCTS (Protected endpoint: [Authorize(Policy = "AdminOnly")])
   * Requires Admin or Employee role in JWT Token!
   */
  getAll(): Observable<Product[]> {
    return this.http.get<Product[]>(`${this.apiUrl}/Product/GetAll`);
  }

  /**
   * GET BY ID (Protected endpoint: [Authorize(Roles = "Employee")])
   * Requires Employee role in JWT Token!
   */
  getById(id: number): Observable<Product> {
    return this.http.get<Product>(`${this.apiUrl}/Product/${id}`);
  }

  /**
   * CREATE PRODUCT (Protected endpoint: [Authorize(Roles = "Admin")])
   * Requires Admin role in JWT Token!
   */
  create(product: ProductCreate): Observable<Product> {
    return this.http.post<Product>(`${this.apiUrl}/Product/Add`, product);
  }

  /**
   * UPDATE PRODUCT (Protected endpoint: [Authorize(Roles = "User")])
   * Requires User role in JWT Token!
   */
  update(product: ProductUpdate): Observable<any> {
    return this.http.put<any>(`${this.apiUrl}/Product/Update`, product);
  }

  /**
   * DELETE PRODUCT (Protected endpoint: [Authorize(Roles = "Employee")])
   * Requires Employee role in JWT Token!
   */
  delete(id: number): Observable<any> {
    return this.http.delete<any>(`${this.apiUrl}/Product/Delete?id=${id}`);
  }
}
