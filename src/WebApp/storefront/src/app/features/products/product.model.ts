export interface Product {
  id: string;
  name: string;
  category: string[];
  description: string;
  imageFile: string;
  price: number;
}

export interface GetProductsResponse {
  products: Product[];
}

export interface GetProductByIdResponse {
  product: Product;
}

export interface GetProductByCategoryResponse {
  products: Product[];
}

export type CreateProductRequest = Omit<Product, 'id'>;

export interface CreateProductResponse {
  id: string;
}

export type UpdateProductRequest = Product;

export interface UpdateProductResponse {
  isSuccess: boolean;
}

export interface DeleteProductResponse {
  isSuccess: boolean;
}
