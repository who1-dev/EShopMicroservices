import { Injectable, computed, signal } from '@angular/core';
import { Product } from '../products/product.model';

export interface CartItem {
  product: Product;
  quantity: number;
}

// Client-side cart state. Not yet synced with the basket service.
@Injectable({ providedIn: 'root' })
export class CartService {
  private readonly _items = signal<CartItem[]>([]);

  readonly items = this._items.asReadonly();
  readonly count = computed(() => this._items().reduce((sum, item) => sum + item.quantity, 0));
  readonly total = computed(() =>
    this._items().reduce((sum, item) => sum + item.product.price * item.quantity, 0),
  );

  add(product: Product, quantity = 1): void {
    this._items.update((items) => {
      const existing = items.find((item) => item.product.id === product.id);
      return existing
        ? items.map((item) =>
            item === existing ? { ...item, quantity: item.quantity + quantity } : item,
          )
        : [...items, { product, quantity }];
    });
  }

  remove(productId: string): void {
    this._items.update((items) => items.filter((item) => item.product.id !== productId));
  }

  clear(): void {
    this._items.set([]);
  }
}
