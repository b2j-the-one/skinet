import { inject, Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { DeliveryMethod } from '../../shared/models/deliveryMethod';
import { map, of, tap } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class CheckoutService {
  baseUrl = environment.apiUrl;
  private http = inject(HttpClient);

  deliveryMethods: DeliveryMethod[] = [];
  // private deliveryMethods = signal<DeliveryMethod[]>([]);
  // readonly deliveryMethods$ = this.deliveryMethods.asReadonly();

  // /**
  //  * Obtenir les modes de livraison
  //  * Est‑ce que map est conseillé ici ? :
  //  * - Oui, mais seulement pour la partie transformation (le tri -> methods.sort(...))
  //  * - Non, pour la partie side‑effect (mettre en cache -> this.deliveryMethods = ...)
  //  *
  //  * @returns Un observable des modes de livraison
  //  */
  // getDeliveryMethods() {
  //   if (this.deliveryMethods.length > 0) return of(this.deliveryMethods);
  //   return this.http.get<DeliveryMethod[]>(this.baseUrl + 'payments/delivery-methods').pipe(
  //     map((methods) => {
  //       this.deliveryMethods = methods.sort((a, b) => b.price - a.price);
  //       return methods;
  //     }),
  //   );
  // }

  /**
   * Obtenir les modes de livraison
   * Ce code est plus lisible car on voit immédiatement :
   * - ce qui transforme la donnée (map)
   * - ce qui met à jour l’état interne du service (tap)
   *
   * @returns Les modes de livraison (observable)
   */
  getDeliveryMethods() {
    // Si déjà en cache → renvoyer immédiatement
    if (this.deliveryMethods.length > 0) return of(this.deliveryMethods);
    // Sinon → appel HTTP
    return this.http.get<DeliveryMethod[]>(this.baseUrl + 'payments/delivery-methods').pipe(
      map((methods) => methods.sort((a, b) => b.price - a.price)), // transformation (le tri)
      tap((sorted) => (this.deliveryMethods = sorted)), // side-effect (mettre en cache la valeur triée)
    );
  }

  // /**
  //  * Obtenir les modes de livraison
  //  * Ce code est plus lisible car on voit immédiatement :
  //  * - ce qui transforme la donnée (map)
  //  * - ce qui met à jour l’état interne du service (tap)
  //  *
  //  * @returns Un observable des modes de livraison
  //  */
  // loadDeliveryMethods() {
  //   return this.http.get<DeliveryMethod[]>(this.baseUrl + 'payments/delivery-methods').pipe(
  //     map((methods) => methods.sort((a, b) => b.price - a.price)), // transformation (le tri)
  //     tap((sorted) => this.deliveryMethods.set(sorted)), // side-effect (mettre en cache la valeur triée)
  //   );
  // }

  // getDeliveryMethods() {
  //   return this.deliveryMethods().length > 0
  //     ? of(this.deliveryMethods())
  //     : this.loadDeliveryMethods();
  // }
}
