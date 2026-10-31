import { Injectable, inject, signal } from '@angular/core';
import { HttpClient, httpResource } from '@angular/common/http';
import { Observable, tap } from 'rxjs';
import {
  CreateProductRequest,
  CreateProductResponse,
  DeleteProductResponse,
  GetProductByCategoryResponse,
  GetProductByIdResponse,
  GetProductsResponse,
  UpdateProductRequest,
  UpdateProductResponse,
} from './product.model';

const BASE_URL = '/catalog-service/products';

@Injectable({ providedIn: 'root' })
export class ProductsService {
  private readonly http = inject(HttpClient);

  readonly pageNumber = signal(1);
  readonly pageSize = signal(10);

  // GET /products?pageNumber=&pageSize=
  readonly products = httpResource<GetProductsResponse>(
    () => ({
      url: BASE_URL,
      params: { pageNumber: this.pageNumber(), pageSize: this.pageSize() },
    }),
    { defaultValue: { products: [] } },
  );

  // GET /products/{id} — call from an injection context (e.g. a component field initializer).
  productById(id: () => string | undefined) {
    return httpResource<GetProductByIdResponse>(() => {
      const value = id();
      return value ? `${BASE_URL}/${value}` : undefined;
    });
  }

  // GET /products/category/{category} — call from an injection context.
  productsByCategory(category: () => string | undefined) {
    return httpResource<GetProductByCategoryResponse>(
      () => {
        const value = category();
        return value ? `${BASE_URL}/category/${encodeURIComponent(value)}` : undefined;
      },
      { defaultValue: { products: [] } },
    );
  }

  // POST /products
  create(request: CreateProductRequest): Observable<CreateProductResponse> {
    return this.http
      .post<CreateProductResponse>(BASE_URL, request)
      .pipe(tap(() => this.products.reload()));
  }

  // PUT /products
  update(request: UpdateProductRequest): Observable<UpdateProductResponse> {
    return this.http
      .put<UpdateProductResponse>(BASE_URL, request)
      .pipe(tap(() => this.products.reload()));
  }

  // DELETE /products/{id}
  delete(id: string): Observable<DeleteProductResponse> {
    return this.http
      .delete<DeleteProductResponse>(`${BASE_URL}/${id}`)
      .pipe(tap(() => this.products.reload()));
  }
}
