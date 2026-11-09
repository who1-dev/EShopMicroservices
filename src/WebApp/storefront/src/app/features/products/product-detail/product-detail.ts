import { Component, computed, effect, inject, input, signal } from '@angular/core';
import { CurrencyPipe } from '@angular/common';
import { RouterLink } from '@angular/router';
import { ApiError } from '../../../core/http/api-error';
import { BannerService } from '../../../shared/banner/banner.service';
import { CartService } from '../../cart/cart.service';
import { PRODUCT_COLORS, Product, ProductColor } from '../product.model';
import { ProductsService } from '../products.service';
import { ProductImagePipe } from '../product-image.pipe';

@Component({
  selector: 'app-product-detail',
  imports: [CurrencyPipe, ProductImagePipe, RouterLink],
  templateUrl: './product-detail.html',
  styleUrl: './product-detail.scss',
})
export class ProductDetail {
  private readonly productsService = inject(ProductsService);
  private readonly cartService = inject(CartService);
  private readonly bannerService = inject(BannerService);
  private errorBannerId: number | null = null;

  protected readonly colors = PRODUCT_COLORS;
  protected readonly maxQuantity = 99;

  // Route param from /products/:id, bound via withComponentInputBinding().
  readonly id = input<string>();

  protected readonly resource = this.productsService.productById(this.id);
  protected readonly imageBroken = signal(false);
  protected readonly color = signal<ProductColor>('Black');
  protected readonly quantity = signal(1);

  // value() throws while the resource is in an error state, so guard with hasValue().
  protected readonly product = computed(() =>
    this.resource.hasValue() ? this.resource.value().product : undefined,
  );

  protected readonly notFound = computed(() => {
    const error = this.resource.error();
    return error instanceof ApiError && error.status === 404;
  });

  constructor() {
    // A new id means a new product; reset the image fallback and the selected options.
    effect(() => {
      this.id();
      this.imageBroken.set(false);
      this.color.set('Black');
      this.quantity.set(1);
    });

    // Show load failures in the app banner. 404 is handled inline by the template.
    effect(() => {
      const error = this.resource.error();
      if (this.errorBannerId !== null) {
        this.bannerService.dismiss(this.errorBannerId);
        this.errorBannerId = null;
      }
      if (error && !this.notFound()) {
        this.errorBannerId = this.bannerService.error(
          error instanceof ApiError ? error : `Could not load product details. ${error.message}`,
        );
      }
    });
  }

  protected changeQuantity(delta: number): void {
    this.quantity.update((q) => this.clampQuantity(q + delta));
  }

  // Typed values are clamped to 1..maxQuantity; empty or invalid input falls back to 1.
  protected onQuantityChange(event: Event): void {
    const input = event.target as HTMLInputElement;
    const quantity = this.clampQuantity(input.valueAsNumber);
    this.quantity.set(quantity);
    // If the clamped value equals the current one, the signal doesn't change, so fix the field here.
    input.value = String(quantity);
  }

  protected addToCart(product: Product): void {
    const quantity = this.quantity();
    const color = this.color();
    this.cartService.add(product, color, quantity);
    this.bannerService.success(`${quantity} × ${product.name} (${color}) added to cart.`);
  }

  private clampQuantity(value: number): number {
    if (!Number.isFinite(value)) return 1;
    return Math.min(this.maxQuantity, Math.max(1, Math.trunc(value)));
  }
}
