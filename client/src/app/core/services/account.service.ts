import { inject, Injectable, signal } from '@angular/core';
import { environment } from '../../../environments/environment';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Address, User } from '../../shared/models/user';
import { map, tap } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class AccountService {
  baseUrl = environment.apiUrl;
  private http = inject(HttpClient);
  currentUser = signal<User | null>(null);

  login(values: any) {
    // let params = new HttpParams();
    // params = params.append('useCookies', true);
    // return this.http.post<User>(this.baseUrl + 'login', values, { params });
    return this.http.post<User>(this.baseUrl + 'account/login', values);
  }

  register(values: any) {
    return this.http.post(this.baseUrl + 'account/register', values);
  }

  // /**
  //  * Obtenir les infos de l'utilisateur connecté
  //  * Mettre à jour un signal (this.currentUser.set(user)) est un side‑effect, donc tap est l’opérateur correct.
  //  * @returns
  //  */
  // getUserInfo() {
  //   return this.http.get<User>(this.baseUrl + 'account/user-info').pipe(
  //     map((user) => {
  //       this.currentUser.set(user);
  //       return user;
  //     }),
  //   );
  // }

  /**
   * Obtenir les infos de l'utilisateur connecté
   * Cette méthode est mieux car :
   * - tap est fait pour les side-effects
   * - le flux reste propre
   * - on ne mélanges pas transformation et mutation d’état
   * - c’est la convention Angular + RxJS moderne
   *
   * Un side‑effect (effet secondaire ou action externe au flux) = une action déclenchée par le flux (Observable), mais qui ne modifie pas la valeur du flux.
   * Exemples typiques :
   * mettre à jour un signal Angular
   * écrire dans le localStorage
   * afficher un log
   * déclencher une navigation
   * appeler une autre méthode
   * mettre à jour un store NgRx / signalStore
   * envoyer un event analytics
   *
   * @returns  L'utilisateur connecté
   */
  getUserInfo() {
    return this.http.get<User>(this.baseUrl + 'account/user-info').pipe(
      tap((user) => {
        // Ici, on modifies l’état de l'application, pas la donnée du flux.
        // Le flux continue d’émettre la même valeur.
        // On fait juste une action à côté → side‑effect.
        this.currentUser.set(user);
      }),
    );
  }

  /**
   * Voici une utilisation cohérente de map et tap
   * - map transforme
   * - tap met à jour le signal (side-effect)
   *
   * @returns L'utilisateur connecté
   */
  getUserInfo1() {
    return this.http.get<User>(this.baseUrl + 'account/user-info').pipe(
      map((user) => ({
        ...user,
        fullName: user.firstName + ' ' + user.lastName,
      })),
      tap((user) => this.currentUser.set(user)),
    );
  }

  logout() {
    return this.http.post(this.baseUrl + 'account/logout', {});
  }

  // /**
  //  * Mettre à jour l'adresse de l'utilisateur
  //  * @param address
  //  * @returns L'Address de l'utilisateur
  //  */
  // updateAddress(address: Address) {
  //   return this.http.post<Address>(this.baseUrl + 'account/address', address).pipe(
  //     tap(() => {
  //       this.currentUser.update((user) => {
  //         if (user) {
  //           user.address = address; // C’est une mutation, ce qui est déconseillé avec les signals
  //         }
  //         return user;
  //       });
  //     }),
  //   );
  // }

  /**
   * Mettre à jour l'adresse de l'utilisateur
   * Cette méthode est meilleur car on utilise la valeur retournée par l’API.
   * Donc côté Angular, autant utiliser la vraie valeur renvoyée par le serveur, pas celle envoyée par le client.
   * Ça évite les incohérences si le backend modifie quelque chose (normalisation, ID, etc.).
   *
   * @param address
   * @returns L'Address de l'utilisateur
   */
  updateAddress(address: Address) {
    return this.http.post<Address>(this.baseUrl + 'account/address', address).pipe(
      tap((updatedAddress) => {
        // { ...user, address: updatedAddress } : Nouvel objet → meilleur pour le change detection et la stabilité.
        this.currentUser.update((user) => (user ? { ...user, address: updatedAddress } : user));
      }),
    );
  }

  /**
   * Vérifier si l'utilisateur est authentifié
   * @returns Vrai si l'utilisateur est authentifié, faux sinon
   */
  getAuthState() {
    return this.http.get<{ isAuthenticated: boolean }>(this.baseUrl + 'account/auth-status');
  }
}
