import { inject, Injectable } from '@angular/core';
import { CartService } from './cart.service';
import { catchError, forkJoin, of, tap } from 'rxjs';
import { AccountService } from './account.service';
import { SignalrService } from './signalr.service';

@Injectable({
  providedIn: 'root',
})
export class InitService {
  private cartService = inject(CartService);
  private accountService = inject(AccountService);
  private signalrService = inject(SignalrService);

  init() {
    const cartId = localStorage.getItem('cart_id');

    const cart$ = cartId
      ? this.cartService.getCart(cartId).pipe(catchError(() => of(null)))
      : of(null);

    const user$ = this.accountService.getUserInfo().pipe(
      tap((user) => {
        if (user) this.signalrService.createHubConnection();
      }),
      catchError(() => of(null)), // Utilisateur non connecté
    );

    /**
     * forkJoin
     * Prend plusieurs Observables (tableau ou objet) et attend leur completion
     * Émet une seule fois : soit un tableau des dernières valeurs de chaque Observable, soit un objet si tu lui passes un objet.
     * Si un seul Observable échoue, forkJoin échoue entièrement.
     */
    return forkJoin({ cart: cart$, user: user$ });
  }
}
