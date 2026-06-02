namespace Core.Entities.OrderAggregate;

public class PaymentSummary
{
    /// <summary>
    /// Les 4 derniers chiffres de la carte
    /// </summary>
    public int Last4 { get; set; }
    /// <summary>
    /// La marque de la carte
    /// </summary>
    public required string Brand { get; set; }
    /// <summary>
    /// Le mois d'expiration de la carte
    /// </summary>
    public int ExpMonth { get; set; }
    /// <summary>
    /// L'année d'expiration de la carte
    /// </summary>
    public int Year { get; set; }
}
