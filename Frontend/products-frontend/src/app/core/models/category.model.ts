/**
 * Espejo de DTOs/CategoryDto.cs
 */

// GET /api/categories y GET /api/categories/{id} devuelven esto
export interface Category {
  id: number;
  name: string;
  description: string | null;
  productCount: number;
}

// Body que se envía en POST /api/categories
export interface CategoryCreate {
  name: string;
  description?: string | null;
}

// Body que se envía en PUT /api/categories/{id}
export interface CategoryUpdate {
  name: string;
  description?: string | null;
}