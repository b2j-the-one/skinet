namespace Core.Entities.OrderAggregate;

public enum OrderStatus
{
    /// <summary>En attante</summary>
    Pending,
    /// <summary>Paiement reçu</summary>
    PaymentReceveid,
    /// <summary>Paiement échoué</summary>
    PaymentFailed
}
