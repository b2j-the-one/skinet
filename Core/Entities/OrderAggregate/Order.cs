namespace Core.Entities.OrderAggregate;

public class Order : BaseEntity
{
    /// <summary>
    /// La date de la commande
    /// </summary>
    public DateTime OrderDate { get; set; } = DateTime.UtcNow;
    /// <summary>
    /// L'email de l'acheteur
    /// </summary>
    public required string BuyerEmail { get; set; }
    /// <summary>
    /// L'adresse de livraison
    /// </summary>
    public ShippingAddress ShippingAddress { get; set; } = null!;
    /// <summary>
    /// Le mode de livraison
    /// </summary>
    public DeliveryMethod DeliveryMethod { get; set; } = null!;
    /// <summary>
    /// Le récapitulatif du paiement
    /// </summary>
    public PaymentSummary PaymentSummary { get; set; } = null!;
    /// <summary>
    /// Les articles de la commande
    /// </summary>
    public IReadOnlyList<OrderItem> OrderItems { get; set; } = [];
    /// <summary>
    /// Le sous total de la commande
    /// </summary>
    public decimal SubTotal { get; set; }
    /// <summary>
    /// Le statut du paiement
    /// </summary>
    public OrderStatus Status { get; set; }
    /// <summary>
    /// L'dentifiant de l'élément de paiement (Stripe)
    /// </summary>
    public required string PaymentItentId { get; set; }
}
