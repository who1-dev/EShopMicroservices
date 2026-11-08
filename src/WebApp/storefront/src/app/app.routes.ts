import { Routes } from '@angular/router';

const productList = () =>
  import('./features/products/product-list/product-list').then((m) => m.ProductList);
const placeholder = () => import('./shared/placeholder/placeholder').then((m) => m.Placeholder);

const productDetail = () => 
  import('./features/products/product-detail/product-detail').then((m) => m.ProductDetail);


export const routes: Routes = [
  { path: '', loadComponent: productList },
  { path: 'products', loadComponent: productList },
  { path: 'products/:id', loadComponent: productDetail},
  { path: 'cart', loadComponent: placeholder, data: { title: 'Cart' } },
  { path: 'orders', loadComponent: placeholder, data: { title: 'Orders' } },
  { path: 'contact', loadComponent: placeholder, data: { title: 'Contact' } },
  { path: '**', redirectTo: '' },
];
