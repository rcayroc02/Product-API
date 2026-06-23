import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ApiResponse, PaginatedResponse } from '../models/api-response.model';
import { Product, ProductCreate, ProductUpdate } from '../models/product.model';

@Injectable({
  providedIn: 'root'
})
export class ProductService {
  private http = inject(HttpClient);
  private baseUrl = `${environment.apiUrl}/products`;

  getProducts(
    page: number = 1,
    pageSize: number = 10,
    categoryId?: number
  ): Observable<ApiResponse<PaginatedResponse<Product>>> {
    let params = new HttpParams()
      .set('page', page)
      .set('pageSize', pageSize);

    if (categoryId !== undefined && categoryId !== null) {
      params = params.set('category', categoryId);
    }

    return this.http.get<ApiResponse<PaginatedResponse<Product>>>(this.baseUrl, { params });
  }

  getProduct(id: number): Observable<ApiResponse<Product>> {
    return this.http.get<ApiResponse<Product>>(`${this.baseUrl}/${id}`);
  }

  createProduct(dto: ProductCreate): Observable<ApiResponse<Product>> {
    return this.http.post<ApiResponse<Product>>(this.baseUrl, dto);
  }

  updateProduct(id: number, dto: ProductUpdate): Observable<ApiResponse<object>> {
    return this.http.put<ApiResponse<object>>(`${this.baseUrl}/${id}`, dto);
  }

  deleteProduct(id: number): Observable<ApiResponse<object>> {
    return this.http.delete<ApiResponse<object>>(`${this.baseUrl}/${id}`);
  }

  askChatbot(query: string): Observable<ApiResponse<Product[]>> {
    const params = new HttpParams().set('query', query);
    return this.http.get<ApiResponse<Product[]>>(`${this.baseUrl}/chat`, { params });
  }
}