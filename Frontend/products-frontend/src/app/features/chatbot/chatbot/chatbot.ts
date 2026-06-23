import { Component, inject, signal, ViewChild, ElementRef, AfterViewChecked } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ProductService } from '../../../core/services/product';
import { Product } from '../../../core/models/product.model';
import { AppError } from '../../../core/models/app-error.model';

interface ChatMessage {
  id: number;
  sender: 'user' | 'bot';
  text?: string;
  products?: Product[];
  isError?: boolean;
  isLoading?: boolean;
}

@Component({
  selector: 'app-chatbot',
  imports: [CommonModule, FormsModule],
  templateUrl: './chatbot.html',
  styleUrl: './chatbot.scss'
})
export class Chatbot implements AfterViewChecked {
  private productService = inject(ProductService);

  @ViewChild('scrollAnchor') private scrollAnchor?: ElementRef<HTMLDivElement>;

  messages = signal<ChatMessage[]>([
    {
      id: 0,
      sender: 'bot',
      text: 'Hola, soy tu asistente de productos. Pregúntame cosas como "laptops menores a 1500" o "ropa con stock disponible".'
    }
  ]);

  currentQuery = signal('');
  isWaitingResponse = signal(false);

  private nextId = 1;
  private shouldScroll = false;

  ngAfterViewChecked(): void {
    if (this.shouldScroll) {
      this.scrollAnchor?.nativeElement.scrollIntoView({ behavior: 'smooth' });
      this.shouldScroll = false;
    }
  }

  sendMessage(): void {
    const query = this.currentQuery().trim();
    if (!query || this.isWaitingResponse()) return;

    this.messages.update(msgs => [
      ...msgs,
      { id: this.nextId++, sender: 'user', text: query }
    ]);

    this.currentQuery.set('');
    this.isWaitingResponse.set(true);
    this.shouldScroll = true;

    this.productService.askChatbot(query).subscribe({
      next: (response) => {
        this.isWaitingResponse.set(false);
        const products = response.data ?? [];

        this.messages.update(msgs => [
          ...msgs,
          {
            id: this.nextId++,
            sender: 'bot',
            text: products.length > 0
              ? `Encontré ${products.length} producto(s):`
              : 'No encontré productos que coincidan con tu búsqueda.',
            products
          }
        ]);
        this.shouldScroll = true;
      },
      error: (err: AppError) => {
        this.isWaitingResponse.set(false);
        this.messages.update(msgs => [
          ...msgs,
          {
            id: this.nextId++,
            sender: 'bot',
            text: err.message || 'Ocurrió un error al consultar el asistente.',
            isError: true
          }
        ]);
        this.shouldScroll = true;
      }
    });
  }

  onEnterKey(event: Event): void {
    const keyboardEvent = event as KeyboardEvent;
    if (!keyboardEvent.shiftKey) {
      event.preventDefault();
      this.sendMessage();
    }
  }
}