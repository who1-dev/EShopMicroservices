import { InjectionToken } from '@angular/core';
import { environment } from '../../../environments/environment';

/**
 * Base URL for product images (local /images, an S3 bucket, Azure Blob container, CDN, ...).
 * Defaults to the environment value; override with a provider in app.config.ts if needed.
 */
export const IMAGE_BASE_URL = new InjectionToken<string>('IMAGE_BASE_URL', {
  providedIn: 'root',
  factory: () => environment.imageBaseUrl,
});
