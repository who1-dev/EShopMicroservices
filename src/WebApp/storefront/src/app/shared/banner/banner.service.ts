import { Injectable, signal } from '@angular/core';
import { ApiError } from '../../core/http/api-error';
import { BannerType } from './banner';

export interface BannerMessage {
  id: number;
  type: BannerType;
  message: string;
  details: string[];
}

const SUCCESS_TIMEOUT_MS = 5000;

// App-wide banners rendered by the root component. Success banners auto-dismiss; errors stay until closed.
@Injectable({ providedIn: 'root' })
export class BannerService {
  private nextId = 0;
  private readonly _banners = signal<BannerMessage[]>([]);

  readonly banners = this._banners.asReadonly();

  // Both return the banner id so callers can dismiss it themselves (e.g. on retry).
  success(message: string, details: string[] = []): number {
    const id = this.show('success', message, details);
    setTimeout(() => this.dismiss(id), SUCCESS_TIMEOUT_MS);
    return id;
  }

  error(error: ApiError | string, details: string[] = []): number {
    if (error instanceof ApiError) {
      return this.show('error', error.message, error.validationErrors.map((e) => e.errorMessage));
    }
    return this.show('error', error, details);
  }

  dismiss(id: number): void {
    this._banners.update((banners) => banners.filter((b) => b.id !== id));
  }

  clear(): void {
    this._banners.set([]);
  }

  private show(type: BannerType, message: string, details: string[]): number {
    const id = this.nextId++;
    this._banners.update((banners) => [...banners, { id, type, message, details }]);
    return id;
  }
}
