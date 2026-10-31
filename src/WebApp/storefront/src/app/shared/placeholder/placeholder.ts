import { Component, input } from '@angular/core';

// Stand-in for pages that are not built yet. The title comes from the route's data.
@Component({
  selector: 'app-placeholder',
  template: `
    <section class="placeholder">
      <h1>{{ title() }}</h1>
      <p>This page is coming soon.</p>
    </section>
  `,
  styles: `
    .placeholder {
      max-width: 1200px;
      margin: 0 auto;
      padding: 4rem 1rem;
      text-align: center;
      font-family: system-ui, -apple-system, 'Segoe UI', Roboto, sans-serif;
      color: oklch(50% 0.015 300);
    }

    h1 {
      color: oklch(20% 0.01 300);
    }
  `,
})
export class Placeholder {
  readonly title = input('');
}
