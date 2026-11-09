import { Component, input } from '@angular/core';
import { RouterLink } from '@angular/router';

// Stand-in for pages that are not built yet. The title comes from the route's data.
@Component({
  selector: 'app-placeholder',
  imports: [RouterLink],
  template: `
    <section class="status placeholder container">
      <p class="eyebrow">Coming soon</p>
      <h1>{{ title() }}</h1>
      <p>We're putting the finishing touches on this page.</p>
      <a class="btn" routerLink="/products">Continue shopping</a>
    </section>
  `,
  // Layout and colors come from the global .status class; only the larger scale is set here.
  styles: `
    .placeholder {
      padding-block: clamp(4rem, 12vw, 8rem);
    }

    h1 {
      font-size: clamp(2.5rem, 6vw, 4rem);
    }

    .btn {
      margin-top: 1rem;
    }
  `,
})
export class Placeholder {
  readonly title = input('');
}
