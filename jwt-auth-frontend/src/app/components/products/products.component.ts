import { Component, inject, OnInit, ChangeDetectorRef } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ProductService } from '../../services/product.service';
import { AuthService } from '../../services/auth.service';
import { Product } from '../../models/product.model';

@Component({
  selector: 'app-products',
  imports: [FormsModule],
  templateUrl: './products.component.html',
  styleUrl: './products.component.css'
})
export class ProductsComponent implements OnInit {
  private productService = inject(ProductService);
  private cdr = inject(ChangeDetectorRef);
  authService = inject(AuthService);

  // Products list
  products: Product[] = [];

  // Form model for Add / Edit
  productForm = {
    id: 0,
    name: '',
    description: ''
  };

  isEditMode = false;
  isLoading = false;
  successMessage = '';
  errorMessage = '';

  ngOnInit(): void {
    this.getProducts();
  }

  // 1. READ: Get all products
  getProducts(): void {
    this.isLoading = true;
    this.clearMessages();
    this.cdr.detectChanges();

    this.productService.getAll().subscribe({
      next: (data) => {
        this.products = data || [];
        this.isLoading = false;
        this.cdr.detectChanges();
      },
      error: (err) => {
        this.isLoading = false;
        this.handleError(err, 'fetching products');
        this.cdr.detectChanges();
      }
    });
  }

  // 2. CREATE or UPDATE: Save product
  saveProduct(): void {
    if (!this.productForm.name) {
      this.errorMessage = 'Please enter product name.';
      this.cdr.detectChanges();
      return;
    }

    this.isLoading = true;
    this.clearMessages();
    this.cdr.detectChanges();

    if (this.isEditMode) {
      // UPDATE
      this.productService.update({
        id: this.productForm.id,
        name: this.productForm.name,
        description: this.productForm.description
      }).subscribe({
        next: () => {
          this.successMessage = 'Product updated successfully!';
          this.resetForm();
          this.getProducts();
        },
        error: (err) => {
          this.isLoading = false;
          this.handleError(err, 'updating product');
          this.cdr.detectChanges();
        }
      });
    } else {
      // CREATE
      this.productService.create({
        name: this.productForm.name,
        description: this.productForm.description
      }).subscribe({
        next: () => {
          this.successMessage = 'Product added successfully!';
          this.resetForm();
          this.getProducts();
        },
        error: (err) => {
          this.isLoading = false;
          this.handleError(err, 'adding product');
          this.cdr.detectChanges();
        }
      });
    }
  }

  // Populate form for Edit
  editProduct(item: Product): void {
    this.clearMessages();
    this.isEditMode = true;
    this.productForm = {
      id: item.id,
      name: item.name,
      description: item.description
    };
    this.cdr.detectChanges();
  }

  // 3. DELETE: Remove product
  deleteProduct(id: number): void {
    if (!confirm('Are you sure you want to delete this product?')) {
      return;
    }

    this.isLoading = true;
    this.clearMessages();
    this.cdr.detectChanges();

    this.productService.delete(id).subscribe({
      next: () => {
        this.successMessage = 'Product deleted successfully!';
        this.getProducts();
      },
      error: (err) => {
        this.isLoading = false;
        this.handleError(err, 'deleting product');
        this.cdr.detectChanges();
      }
    });
  }

  // Reset form to Add mode
  resetForm(): void {
    this.isEditMode = false;
    this.productForm = {
      id: 0,
      name: '',
      description: ''
    };
    this.cdr.detectChanges();
  }

  private clearMessages(): void {
    this.successMessage = '';
    this.errorMessage = '';
  }

  private handleError(err: any, action: string): void {
    if (err.status === 403) {
      this.errorMessage = `Permission Denied (403 Forbidden): In .NET controller, this action requires a different role. Your current role is "${this.authService.userRole()}".`;
    } else if (err.status === 401) {
      this.errorMessage = 'Session expired or invalid token. Please log in again.';
    } else {
      this.errorMessage = `Error while ${action}: ${err.error?.message || err.message || 'Server error'}`;
    }
  }
}
