/**
 * Espejo de DTOs/ProductDto.cs
 */

// Lo que devuelve el backend en GET /api/products y GET /api/products/{id}
// (va dentro de ApiResponse<Product> o de PaginatedResponse<Product>)
export interface Product {
  id: number;
  name: string;
  description: string | null;
  price: number;
  stock: number;
  isActive: boolean;
  createdAt: string;      // ISO string, NO Date — convertir solo al mostrar
  categoryName: string;
}

// Body que se envía en POST /api/products
export interface ProductCreate {
  name: string;
  description?: string | null;
  price: number;
  stock: number;
  categoryId: number;
}

// Body que se envía en PUT /api/products/{id}
// Nota: a diferencia de Create, Update SÍ incluye isActive
// (mira ProductUpdateDto en C#: tiene IsActive, ProductCreateDto no)
export interface ProductUpdate {
  name: string;
  description?: string | null;
  price: number;
  stock: number;
  isActive: boolean;
  categoryId: number;
}