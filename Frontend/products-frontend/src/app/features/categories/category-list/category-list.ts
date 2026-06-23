import { Component, inject, signal, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CategoryService } from '../../../core/services/category';
import { Category } from '../../../core/models/category.model';
import { AppError } from '../../../core/models/app-error.model';
import { CategoryForm } from '../category-form/category-form';

type ViewMode = 'list' | 'create' | 'edit';

@Component({
  selector: 'app-category-list',
  imports: [CommonModule, CategoryForm],
  templateUrl: './category-list.html',
  styleUrl: './category-list.scss'
})
export class CategoryList implements OnInit {
  private categoryService = inject(CategoryService);

  categories = signal<Category[]>([]);
  isLoading = signal(false);
  loadError = signal<AppError | null>(null);

  viewMode = signal<ViewMode>('list');
  categoryBeingEdited = signal<Category | null>(null);

  categoryPendingDelete = signal<Category | null>(null);
  deleteError = signal<AppError | null>(null);

  ngOnInit(): void {
    this.loadCategories();
  }

  loadCategories(): void {
    this.isLoading.set(true);
    this.loadError.set(null);

    this.categoryService.getCategories().subscribe({
      next: (cats) => {
        this.isLoading.set(false);
        this.categories.set(cats);
      },
      error: (err: AppError) => {
        this.isLoading.set(false);
        this.loadError.set(err);
      }
    });
  }

  openCreateForm(): void {
    this.categoryBeingEdited.set(null);
    this.viewMode.set('create');
  }

  openEditForm(category: Category): void {
    this.categoryBeingEdited.set(category);
    this.viewMode.set('edit');
  }

  onFormSaved(): void {
    this.viewMode.set('list');
    this.categoryBeingEdited.set(null);
    this.loadCategories();
  }

  onFormCancelled(): void {
    this.viewMode.set('list');
    this.categoryBeingEdited.set(null);
  }

  askDeleteConfirmation(category: Category): void {
    this.categoryPendingDelete.set(category);
    this.deleteError.set(null);
  }

  cancelDelete(): void {
    this.categoryPendingDelete.set(null);
  }

  confirmDelete(): void {
    const category = this.categoryPendingDelete();
    if (!category) return;

    this.categoryService.deleteCategory(category.id).subscribe({
      next: () => {
        this.categoryPendingDelete.set(null);
        this.loadCategories();
      },
      error: (err: AppError) => {
        // Aquí cae el caso "no se puede borrar, tiene productos asociados"
        this.deleteError.set(err);
      }
    });
  }
}