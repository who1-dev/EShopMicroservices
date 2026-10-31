import { Component, inject, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { Banner } from './shared/banner/banner';
import { Navbar } from './shared/navbar/navbar';
import { BannerService } from './shared/banner/banner.service';

@Component({
  imports: [RouterOutlet, Banner, Navbar],
  selector: 'app-root',
  styleUrl: './app.scss',
  templateUrl: './app.html',
})
export class App {
  protected readonly title = signal('storefront');
  protected readonly bannerService = inject(BannerService);
}
