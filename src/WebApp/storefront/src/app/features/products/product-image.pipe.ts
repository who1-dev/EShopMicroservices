import { Pipe, PipeTransform, inject } from '@angular/core';
import { IMAGE_BASE_URL } from '../../core/config/image-base-url';

const ABSOLUTE_URL = /^(?:[a-z][a-z\d+\-.]*:)?\/\//i;

/** Resolves a product's imageFile against IMAGE_BASE_URL. Absolute URLs pass through unchanged. */
@Pipe({ name: 'productImage' })
export class ProductImagePipe implements PipeTransform {
  private readonly baseUrl = inject(IMAGE_BASE_URL).replace(/\/+$/, '');

  transform(imageFile: string | null | undefined): string {
    if (!imageFile) return '';
    if (ABSOLUTE_URL.test(imageFile)) return imageFile;
    return `${this.baseUrl}/${imageFile.replace(/^\/+/, '')}`;
  }
}
