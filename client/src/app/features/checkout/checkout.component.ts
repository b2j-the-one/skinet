import { Component, inject, OnDestroy, OnInit, signal } from '@angular/core';
import { OrderSummaryComponent } from '../../shared/components/order-summary/order-summary.component';
import { MatStepper, MatStepperModule } from '@angular/material/stepper';
import { MatButton } from '@angular/material/button';
import { Router, RouterLink } from '@angular/router';
import { StripeService } from '../../core/services/stripe.service';
import {
  ConfirmationToken,
  StripeAddressElement,
  StripeAddressElementChangeEvent,
  StripePaymentElement,
  StripePaymentElementChangeEvent,
} from '@stripe/stripe-js';
import { SnackbarService } from '../../core/services/snackbar.service';
import { MatCheckboxChange, MatCheckboxModule } from '@angular/material/checkbox';
import { StepperSelectionEvent } from '@angular/cdk/stepper';
import { Address } from '../../shared/models/user';
import { AccountService } from '../../core/services/account.service';
import { firstValueFrom } from 'rxjs';
import { CheckoutDeliveryComponent } from './checkout-delivery/checkout-delivery.component';
import { CheckoutReviewComponent } from './checkout-review/checkout-review.component';
import { CartService } from '../../core/services/cart.service';
import { CurrencyPipe, JsonPipe } from '@angular/common';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';

@Component({
  selector: 'app-checkout',
  imports: [
    OrderSummaryComponent,
    MatStepperModule,
    MatButton,
    RouterLink,
    MatCheckboxModule,
    CheckoutDeliveryComponent,
    CheckoutReviewComponent,
    CurrencyPipe,
    MatProgressSpinnerModule
  ],
  templateUrl: './checkout.component.html',
  styleUrl: './checkout.component.scss',
})
export class CheckoutComponent implements OnInit, OnDestroy {
  private stripeService = inject(StripeService);
  private snackbar = inject(SnackbarService);
  private accountService = inject(AccountService);
  private router = inject(Router);
  cartService = inject(CartService);
  addressElement?: StripeAddressElement;
  paymentElement?: StripePaymentElement;
  saveAddress = false;
  completionStatus = signal<{ address: boolean; card: boolean; delivery: boolean }>({
    address: false,
    card: false,
    delivery: false,
  });
  confirmationToken?: ConfirmationToken;
  loading = false;

  // constructor() {
  //   this.handleAddressChange = this.handleAddressChange.bind(this);
  //   this.handlePaymentChange = this.handlePaymentChange.bind(this);
  // }

  async ngOnInit() {
    try {
      this.addressElement = await this.stripeService.createAddressElement();
      this.addressElement.mount('#address-element');
      this.addressElement.on('change', this.handleAddressChange);

      this.paymentElement = await this.stripeService.createPaymentElement();
      this.paymentElement.mount('#payment-element');
      this.paymentElement.on('change', this.handlePaymentChange);
    } catch (error: any) {
      this.snackbar.error(error.message);
    }
  }

  /**
   * Vérifier si l'adresse est renseignée
   *
   * @param event L'événement lié au renseignement de l'adresse
   */
  handleAddressChange = (event: StripeAddressElementChangeEvent) => {
    this.completionStatus.update((state) => {
      state.address = event.complete;
      return state;
    });
  };

  /**
   * Vérifier si le paiement est réalisé
   *
   * @param event L'événement lié au paiement
   */
  handlePaymentChange = (event: StripePaymentElementChangeEvent) => {
    this.completionStatus.update((state) => {
      state.card = event.complete;
      return state;
    });
  };

  /**
   * Vérifier si le mode de livraison est sélectionné
   *
   * @param event L'événement lié à la sélection du mode de livraison
   */
  handleDeliveryChange(event: boolean) {
    this.completionStatus.update((state) => {
      state.delivery = event;
      return state;
    });
  }

  /**
   * Obténir le token de confirmation du paiement
   */
  async getConfirmationToken() {
    try {
      if (Object.values(this.completionStatus()).every((status) => status === true)) {
        const result = await this.stripeService.createConfirmationToken();
        if (result.error) throw new Error(result.error.message);
        this.confirmationToken = result.confirmationToken;
        console.log(this.confirmationToken);
      }
    } catch (error: any) {
      this.snackbar.error(error.message);
    }
  }

  async onStepChange(event: StepperSelectionEvent) {
    // Adresse
    if (event.selectedIndex === 1 && this.saveAddress) {
      const address = await this.getAddressFromStripeAddress();
      address && firstValueFrom(this.accountService.updateAddress(address));
      // if (address) {
      //   firstValueFrom(this.accountService.updateAddress(address));
      // }
    }
    // Livraison
    if (event.selectedIndex === 2) {
      await firstValueFrom(this.stripeService.createOrUpdatePaymentIntent());
    }
    // Confirmation de paiement
    if (event.selectedIndex === 3) {
      await this.getConfirmationToken();
    }
  }

  /**
   * Valider le paiement
   *
   * @param stepper L'étape en cours
   */
  async confirmPayment(stepper: MatStepper) {
    this.loading = true;
    try {
      if (this.confirmationToken) {
        const result = await this.stripeService.confirmPayment(this.confirmationToken);
        if (result.error) {
          throw new Error(result.error.message);
        } else {
          this.cartService.deleteCart();
          this.cartService.selectedDelivery.set(null);
          this.router.navigateByUrl('/checkout/success');
        }
      }
    } catch (error: any) {
      this.snackbar.error(error.message || "Une erreur s'est produite");
      stepper.previous();
    } finally {
      this.loading = false;
    }
  }

  /**
   * Obténir l'adresse via Stripe
   *
   * @returns L'adresse ou null (Promesse)
   */
  private async getAddressFromStripeAddress(): Promise<Address | null> {
    const result = await this.addressElement?.getValue();
    const address = result?.value.address;

    if (address) {
      return {
        line1: address.line1,
        line2: address.line2 || undefined,
        city: address.city,
        country: address.country,
        state: address.state,
        postalCode: address.postal_code,
      };
    } else return null;
  }

  onSaveAddressCheckboxChange(event: MatCheckboxChange) {
    this.saveAddress = event.checked;
  }

  ngOnDestroy(): void {
    this.stripeService.disposeElements();
  }
}
