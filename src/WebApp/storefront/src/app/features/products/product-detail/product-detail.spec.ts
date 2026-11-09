import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { provideRouter } from '@angular/router';
import { ProductDetail } from './product-detail';
import { Product } from '../product.model';
import { apiErrorInterceptor } from '../../../core/http/api-error.interceptor';
import { CartService } from '../../cart/cart.service';

describe('ProductDetail', () => {
  let fixture: ComponentFixture<ProductDetail>;
  let httpTesting: HttpTestingController;

  const product: Product = {
    id: 'abc-123',
    name: 'Test Phone',
    category: ['Smart Phone'],
    description: 'A phone for testing.',
    imageFile: 'product-1.png',
    price: 499,
  };

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [ProductDetail],
      providers: [
        // Same interceptor as app.config.ts, so failed requests surface as ApiError.
        provideHttpClient(withInterceptors([apiErrorInterceptor])),
        provideHttpClientTesting(),
        provideRouter([]),
      ],
    });

    httpTesting = TestBed.inject(HttpTestingController);
    fixture = TestBed.createComponent(ProductDetail);
    fixture.componentRef.setInput('id', product.id);
    fixture.detectChanges();
    TestBed.tick();

    // ProductsService also owns the product-list resource, which loads when the service is created.
    httpTesting
      .match((req) => req.url === '/catalog-service/products')
      .forEach((req) => req.flush({ products: [] }));
  });

  afterEach(() => httpTesting.verify());

  it('fetches the product by id and renders it', async () => {
    httpTesting.expectOne('/catalog-service/products/abc-123').flush({ product });
    await fixture.whenStable();

    const el: HTMLElement = fixture.nativeElement;
    expect(el.querySelector('.detail__title')?.textContent).toContain('Test Phone');
    expect(el.querySelector('.detail__price')?.textContent).toContain('$499.00');
  });

  it('adds the selected color and quantity to the cart', async () => {
    httpTesting.expectOne('/catalog-service/products/abc-123').flush({ product });
    await fixture.whenStable();
    const el: HTMLElement = fixture.nativeElement;

    // Default selection is Black, quantity 1.
    const radios = el.querySelectorAll<HTMLInputElement>('input[name="color"]');
    expect([...radios].map((r) => r.value)).toEqual(['Black', 'White']);
    expect(radios[0].checked).toBe(true);

    radios[1].click();
    await fixture.whenStable();
    const labels = el.querySelectorAll('.swatch');
    expect(radios[1].checked).toBe(true);
    expect(radios[0].checked).toBe(false);
    expect(labels[1].classList).toContain('swatch--active');
    expect(labels[0].classList).not.toContain('swatch--active');
    expect(el.querySelector('legend')?.textContent).toContain('White');

    el.querySelector<HTMLButtonElement>('[aria-label="Increase quantity"]')!.click();
    el.querySelector<HTMLButtonElement>('[aria-label="Increase quantity"]')!.click();
    await fixture.whenStable();

    el.querySelector<HTMLButtonElement>('.detail__button')!.click();

    const items = TestBed.inject(CartService).items();
    expect(items).toEqual([{ product, color: 'White', quantity: 3 }]);
  });

  it('clamps typed quantities to 1..99', async () => {
    httpTesting.expectOne('/catalog-service/products/abc-123').flush({ product });
    await fixture.whenStable();
    const input = fixture.nativeElement.querySelector('#quantity') as HTMLInputElement;

    input.value = '500';
    input.dispatchEvent(new Event('change'));
    await fixture.whenStable();
    expect(input.value).toBe('99');

    input.value = '';
    input.dispatchEvent(new Event('change'));
    await fixture.whenStable();
    expect(input.value).toBe('1');
  });

  it('shows "not found" on a 404', async () => {
    httpTesting
      .expectOne('/catalog-service/products/abc-123')
      .flush({ title: 'Not Found' }, { status: 404, statusText: 'Not Found' });
    await fixture.whenStable();

    expect(fixture.nativeElement.textContent).toContain('Product not found');
  });
});
