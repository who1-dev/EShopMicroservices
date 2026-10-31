import { Component, inject, signal } from '@angular/core';
import { Router, RouterLink, RouterLinkActive } from '@angular/router';
import { CartService } from '../../features/cart/cart.service';

interface NavLink {
  label: string;
  path: string;
  exact: boolean;
}

@Component({
  selector: 'app-navbar',
  imports: [RouterLink, RouterLinkActive],
  templateUrl: './navbar.html',
  styleUrl: './navbar.scss',
})
export class Navbar {
  private readonly router = inject(Router);
  protected readonly cart = inject(CartService);

  protected readonly links: NavLink[] = [
    { label: 'Home', path: '/', exact: true },
    { label: 'Products', path: '/products', exact: false },
    { label: 'Cart', path: '/cart', exact: false },
    { label: 'Orders', path: '/orders', exact: false },
    { label: 'Contact', path: '/contact', exact: false },
  ];

  protected readonly menuOpen = signal(false);

  protected toggleMenu(): void {
    this.menuOpen.update((open) => !open);
  }

  protected closeMenu(): void {
    this.menuOpen.set(false);
  }

  protected search(event: Event, term: string): void {
    event.preventDefault();
    const q = term.trim();
    this.router.navigate(['/products'], { queryParams: { q: q || null } });
    this.closeMenu();
  }
}
