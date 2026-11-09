import { Component, computed, effect, inject, input, signal } from '@angular/core';
import { CurrencyPipe } from '@angular/common';
import { RouterLink } from '@angular/router';
import { ApiError } from '../../../core/http/api-error';
import { BannerService } from '../../../shared/banner/banner.service';
import { ProductsService } from '../products.service';
import { ProductImagePipe } from '../product-image.pipe';

@Component({
  selector: 'app-product-list',
  imports: [CurrencyPipe, ProductImagePipe, RouterLink],
  templateUrl: './product-list.html',
  styleUrl: './product-list.scss',
})
export class ProductList {
  private readonly productsService = inject(ProductsService);
  private readonly bannerService = inject(BannerService);
  private errorBannerId: number | null = null;

  // Search term from the ?q= query param, bound via withComponentInputBinding().
  readonly q = input<string>();

  protected readonly resource = this.productsService.products;
  protected readonly selectedCategory = signal<string | null>(null);
  protected readonly brokenImages = signal(new Set<string>());
  // Number of placeholder cards shown while products load.
  protected readonly skeletons = Array.from({ length: 8 }, (_, i) => i);

  // value() throws while the resource is in an error state, even with a defaultValue.
  protected readonly products = computed(() =>
    this.resource.hasValue() ? this.resource.value().products : [],
  );

  protected readonly categories = computed(() => [
    ...new Set(this.products().flatMap((p) => p.category)),
  ]);

  protected readonly filteredProducts = computed(() => {
    const category = this.selectedCategory();
    const term = this.q()?.trim().toLowerCase();
    return this.products().filter(
      (p) =>
        (!category || p.category.includes(category)) &&
        (!term || p.name.toLowerCase().includes(term)),
    );
  });

  constructor() {
    // Show load failures in the app banner; clear the old one once the resource reloads.
    effect(() => {
      const error = this.resource.error();
      if (this.errorBannerId !== null) {
        this.bannerService.dismiss(this.errorBannerId);
        this.errorBannerId = null;
      }
      if (error) {
        this.errorBannerId = this.bannerService.error(
          error instanceof ApiError ? error : `Could not load products. ${error.message}`,
        );
      }
    });
  }

  protected selectCategory(category: string | null): void {
    this.selectedCategory.set(category);
  }

  protected onImageError(id: string): void {
    this.brokenImages.update((set) => new Set(set).add(id));
  }
}
