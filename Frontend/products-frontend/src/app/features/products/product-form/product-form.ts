import { Component, inject, input, output, signal, computed, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { ProductService } from '../../../core/services/product';
import { CategoryService } from '../../../core/services/category';
import { Category } from '../../../core/models/category.model';
import { Product } from '../../../core/models/product.model';
import { AppError } from '../../../core/models/app-error.model';

@Component({
  selector: 'app-product-form',
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './product-form.html',
  styleUrl: './product-form.scss'
})
export class ProductForm implements OnInit {
  private fb = inject(FormBuilder);
  private productService = inject(ProductService);
  private categoryService = inject(CategoryService);

  // Si viene un producto, es modo edición; si no, es modo creación
  productToEdit = input<Product | null>(null);

  saved = output<void>();
  cancelled = output<void>();

  categories = signal<Category[]>([]);
  isSaving = signal(false);
  serverError = signal<AppError | null>(null);

  isEditMode = computed(() => this.productToEdit() !== null);

  form = this.fb.group({
    name: ['', [Validators.required, Validators.maxLength(150)]],
    description: ['', [Validators.maxLength(500)]],
    price: [0, [Validators.required, Validators.min(0.01)]],
    stock: [0, [Validators.required, Validators.min(0)]],
    categoryId: [null as number | null, [Validators.required]],
    isActive: [true]
  });

  ngOnInit(): void {
    this.categoryService.getCategories().subscribe({
      next: (cats) => this.categories.set(cats),
      error: (err: AppError) => console.error('Error cargando categorías:', err)
    });

    const product = this.productToEdit();
    if (product) {
      // En modo edición necesitamos el categoryId, pero Product solo trae categoryName.
      // Buscamos el id correspondiente una vez que las categorías carguen.
      this.categoryService.getCategories().subscribe({
        next: (cats) => {
          const match = cats.find(c => c.name === product.categoryName);
          this.form.patchValue({
            name: product.name,
            description: product.description ?? '',
            price: product.price,
            stock: product.stock,
            categoryId: match ? match.id : null,
            isActive: product.isActive
          });
        }
      });
    }
  }

  // Helper para el template: ¿este campo tiene un error específico y ya fue tocado?
  hasError(field: string, error: string): boolean {
    const control = this.form.get(field);
    return !!control && control.touched && control.hasError(error);
  }

  onSubmit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.isSaving.set(true);
    this.serverError.set(null);

    const raw = this.form.getRawValue();
    const product = this.productToEdit();

    if (product) {
      // UPDATE: incluye isActive
      this.productService.updateProduct(product.id, {
        name: raw.name!,
        description: raw.description || null,
        price: raw.price!,
        stock: raw.stock!,
        isActive: raw.isActive!,
        categoryId: raw.categoryId!
      }).subscribe({
        next: () => {
          this.isSaving.set(false);
          this.saved.emit();
        },
        error: (err: AppError) => {
          this.isSaving.set(false);
          this.serverError.set(err);
        }
      });
    } else {
      // CREATE: sin isActive (el backend lo pone en true por defecto)
      this.productService.createProduct({
        name: raw.name!,
        description: raw.description || null,
        price: raw.price!,
        stock: raw.stock!,
        categoryId: raw.categoryId!
      }).subscribe({
        next: () => {
          this.isSaving.set(false);
          this.saved.emit();
        },
        error: (err: AppError) => {
          this.isSaving.set(false);
          this.serverError.set(err);
        }
      });
    }
  }

  onCancel(): void {
    this.cancelled.emit();
  }
}