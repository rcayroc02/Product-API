import { Routes } from '@angular/router';
import { ProductList } from './features/products/product-list/product-list';
import { CategoryList } from './features/categories/category-list/category-list';
import { Chatbot } from './features/chatbot/chatbot/chatbot';

export const routes: Routes = [
  { path: '', redirectTo: 'products', pathMatch: 'full' },
  { path: 'products', component: ProductList },
  { path: 'categories', component: CategoryList },
  { path: 'chatbot', component: Chatbot },
  { path: '**', redirectTo: 'products' }
];