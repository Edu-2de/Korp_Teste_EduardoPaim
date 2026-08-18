import { Component, OnInit } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { ProductService } from './core/services/product.service';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet],
  templateUrl: './app.html',
  styleUrl: './app.scss',
})
export class App implements OnInit {
  constructor(private productService: ProductService) {}

  ngOnInit(): void {
    this.productService.getAll().subscribe((products) => {
      console.log('Produtos:', products);
    });
  }
}
