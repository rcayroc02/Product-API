/**
 * Espejo de Models/ApiResponse.cs
 * Envuelve las respuestas de los endpoints de /api/products.
 * OJO: /api/categories NO usa este wrapper, responde el DTO directo.
 */
export interface ApiResponse<T> {
  success: boolean;
  message: string;
  data: T | null;
  errors: string[] | null;
}

/**
 * Espejo de DTOs/ProductDto.cs -> PaginatedResponseDto<T>
 * Es el tipo que viaja DENTRO de ApiResponse<T>.data cuando
 * llamas a GET /api/products (la lista paginada).
 */
export interface PaginatedResponse<T> {
  items: T[];
  totalItems: number;
  page: number;
  pageSize: number;
  totalPages: number;
}