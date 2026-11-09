import { Component, computed, input, output } from '@angular/core';

export type BannerType = 'success' | 'error';

@Component({
  selector: 'app-banner',
  templateUrl: './banner.html',
  styleUrl: './banner.scss',
  host: {
    '[class]': '"banner banner--" + type()',
    '[attr.role]': 'role()',
  },
})
export class Banner {
  readonly type = input<BannerType>('success');
  readonly message = input.required<string>();
  // Extra lines shown under the message, e.g. validation errors from an ApiError.
  readonly details = input<string[]>([]);
  readonly dismissible = input(true);

  readonly dismissed = output<void>();

  // Errors interrupt screen readers; success messages are announced politely.
  protected readonly role = computed(() => (this.type() === 'error' ? 'alert' : 'status'));
}
