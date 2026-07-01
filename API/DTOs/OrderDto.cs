using Core.Entities.OrderAggregate;

namespace API.DTOs;

public class OrderDto
{
    public int Id { get; set; }
    /// <summary>
    /// La date de la commande
    /// </summary>
    public DateTime OrderDate { get; set; }
    /// <summary>
    /// L'email de l'acheteur
    /// </summary>
    public required string BuyerEmail { get; set; }
    /// <summary>
    /// L'adresse de livraison
    /// </summary>
    public required ShippingAddress ShippingAddress { get; set; }
    /// <summary>
    /// Le mode de livraison
    /// </summary>
    public required string DeliveryMethod { get; set; }
    /// <summary>
    /// Frais de livraison
    /// </summary>
    public decimal ShippingPrice { get; set; }
    /// <summary>
    /// Le récapitulatif du paiement
    /// </summary>
    public required PaymentSummary PaymentSummary { get; set; }
    /// <summary>
    /// Les articles de la commande
    /// </summary>
    public required List<OrderItemDto> OrderItems { get; set; }
    /// <summary>
    /// Le sous total de la commande
    /// </summary>
    public decimal SubTotal { get; set; }
    public decimal Total { get; set; }
    /// <summary>
    /// Le statut du paiement
    /// </summary>
    public required string Status { get; set; }
    /// <summary>
    /// L'dentifiant de l'élément de paiement (Stripe)
    /// </summary>
    public required string PaymentItentId { get; set; }
}