import { Injectable, computed, signal } from '@angular/core';
import { Product, ProductColor } from '../products/product.model';

export interface CartItem {
  product: Product;
  color: ProductColor;
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

  // The same product in a different color is a separate cart line.
  add(product: Product, color: ProductColor, quantity = 1): void {
    this._items.update((items) => {
      const existing = items.find((item) => isSameLine(item, product.id, color));
      return existing
        ? items.map((item) =>
            item === existing ? { ...item, quantity: item.quantity + quantity } : item,
          )
        : [...items, { product, color, quantity }];
    });
  }

  remove(productId: string, color: ProductColor): void {
    this._items.update((items) => items.filter((item) => !isSameLine(item, productId, color)));
  }

  clear(): void {
    this._items.set([]);
  }
}

function isSameLine(item: CartItem, productId: string, color: ProductColor): boolean {
  return item.product.id === productId && item.color === color;
}
