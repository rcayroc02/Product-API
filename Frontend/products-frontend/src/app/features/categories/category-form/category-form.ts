import { Component, inject, input, output, signal, computed, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { CategoryService } from '../../../core/services/category';
import { Category } from '../../../core/models/category.model';
import { AppError } from '../../../core/models/app-error.model';

@Component({
  selector: 'app-category-form',
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './category-form.html',
  styleUrl: './category-form.scss'
})
export class CategoryForm implements OnInit {
  private fb = inject(FormBuilder);
  private categoryService = inject(CategoryService);

  categoryToEdit = input<Category | null>(null);

  saved = output<void>();
  cancelled = output<void>();

  isSaving = signal(false);
  serverError = signal<AppError | null>(null);

  isEditMode = computed(() => this.categoryToEdit() !== null);

  form = this.fb.group({
    name: ['', [Validators.required, Validators.maxLength(100)]],
    description: ['', [Validators.maxLength(500)]]
  });

  ngOnInit(): void {
    const category = this.categoryToEdit();
    if (category) {
      this.form.patchValue({
        name: category.name,
        description: category.description ?? ''
      });
    }
  }

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
  const category = this.categoryToEdit();
  const dto = {
    name: raw.name!,
    description: raw.description || null
  };

  if (category) {
    this.categoryService.updateCategory(category.id, dto).subscribe({
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
    this.categoryService.createCategory(dto).subscribe({
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