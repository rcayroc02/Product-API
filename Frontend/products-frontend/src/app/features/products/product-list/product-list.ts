import { Component, inject, signal, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ProductService } from '../../../core/services/product';
import { CategoryService } from '../../../core/services/category';
import { Product } from '../../../core/models/product.model';
import { Category } from '../../../core/models/category.model';
import { AppError } from '../../../core/models/app-error.model';
import { ProductForm } from '../product-form/product-form';

type ViewMode = 'list' | 'create' | 'edit';

@Component({
  selector: 'app-product-list',
  imports: [CommonModule, ProductForm],
  templateUrl: './product-list.html',
  styleUrl: './product-list.scss'
})
export class ProductList implements OnInit {
  private productService = inject(ProductService);
  private categoryService = inject(CategoryService);

  products = signal<Product[]>([]);
  categories = signal<Category[]>([]);
  isLoading = signal(false);
  loadError = signal<AppError | null>(null);

  // Paginación
  page = signal(1);
  pageSize = signal(10);
  totalPages = signal(1);
  totalItems = signal(0);

  // Filtro
  selectedCategoryId = signal<number | null>(null);

  // Control de vista: lista vs. formulario embebido
  viewMode = signal<ViewMode>('list');
  productBeingEdited = signal<Product | null>(null);

  // Confirmación de borrado
  productPendingDelete = signal<Product | null>(null);
  deleteError = signal<AppError | null>(null);

  ngOnInit(): void {
    this.loadCategories();
    this.loadProducts();
  }

  loadCategories(): void {
    this.categoryService.getCategories().subscribe({
      next: (cats) => this.categories.set(cats),
      error: (err: AppError) => console.error('Error cargando categorías:', err)
    });
  }

  loadProducts(): void {
    this.isLoading.set(true);
    this.loadError.set(null);

    const categoryId = this.selectedCategoryId() ?? undefined;

    this.productService.getProducts(this.page(), this.pageSize(), categoryId).subscribe({
      next: (response) => {
        this.isLoading.set(false);
        const data = response.data;
        if (data) {
          this.products.set(data.items);
          this.totalPages.set(data.totalPages);
          this.totalItems.set(data.totalItems);
        }
      },
      error: (err: AppError) => {
        this.isLoading.set(false);
        this.loadError.set(err);
      }
    });
  }

  onCategoryFilterChange(value: string): void {
    this.selectedCategoryId.set(value === '' ? null : Number(value));
    this.page.set(1);
    this.loadProducts();
  }

  goToPage(newPage: number): void {
    if (newPage < 1 || newPage > this.totalPages()) return;
    this.page.set(newPage);
    this.loadProducts();
  }

  openCreateForm(): void {
    this.productBeingEdited.set(null);
    this.viewMode.set('create');
  }

  openEditForm(product: Product): void {
    this.productBeingEdited.set(product);
    this.viewMode.set('edit');
  }

  onFormSaved(): void {
    this.viewMode.set('list');
    this.productBeingEdited.set(null);
    this.loadProducts();
  }

  onFormCancelled(): void {
    this.viewMode.set('list');
    this.productBeingEdited.set(null);
  }

  askDeleteConfirmation(product: Product): void {
    this.productPendingDelete.set(product);
    this.deleteError.set(null);
  }

  cancelDelete(): void {
    this.productPendingDelete.set(null);
  }

  confirmDelete(): void {
    const product = this.productPendingDelete();
    if (!product) return;

    this.productService.deleteProduct(product.id).subscribe({
      next: () => {
        this.productPendingDelete.set(null);
        this.loadProducts();
      },
      error: (err: AppError) => {
        this.deleteError.set(err);
      }
    });
  }
}