using Core.Entities.OrderAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Config;

/// <summary>
/// Définit comment l’entité Order doit être mappée en base de données
/// </summary>
public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        /// ShippingAddress et PaymentSummary ne sont pas des entités séparées
        /// Ce sont des Owned Types : leurs propriétés seront dans la même table Orders
        /// WithOwner() indique que ces objets appartiennent entièrement à Order
        /// Par exemple pour ShippingAddress, EF Core créera les colonnes ShippingAddress_Name, ShippingAddress_Line1 etc. dans la table Order
        builder.OwnsOne(x => x.ShippingAddress, o => o.WithOwner());
        builder.OwnsOne(x => x.PaymentSummary, o => o.WithOwner());

        /// On demandes à EF Core de stocker l’enum sous forme de string dans la base (OrderStatus.Pending → "Pending" etc.)
        /// Coversion : Écriture en base : enum → string | Lecture depuis la base : string → enum
        /// ça permet de : rendre la base plus lisible et éviter les problèmes si l’ordre des enums change
        builder.Property(x => x.Status).HasConversion(
            o => o.ToString(),
            o => (OrderStatus)Enum.Parse(typeof(OrderStatus), o)
        );

        builder.Property(x => x.SubTotal).HasColumnType("decimal(18,2)");

        /// Un Order possède plusieurs OrderItems
        /// OrderItem n’a pas de navigation inverse (WithOne() sans paramètre)
        /// Si un Order est supprimé → tous les OrderItems sont supprimés automatiquement
        /// .WithOne() sans paramètre signifie : L’entité OrderItem n’a PAS de propriété de navigation qui pointe vers Order.
        /// Autrement dit : Order connaît ses OrderItems mais OrderItem ne connaît pas son Order
        builder.HasMany(x => x.OrderItems).WithOne().OnDelete(DeleteBehavior.Cascade);

        /// Cette configuration dit à EF Core :
        /// Quand tu enregistres OrderDate en base, convertis-le en UTC.
        /// Quand tu le relis depuis la base, considère qu’il est en UTC.
        builder.Property(x => x.OrderDate).HasConversion(
            d => d.ToUniversalTime(), // écriture en base
            d => DateTime.SpecifyKind(d, DateTimeKind.Utc) // Lecture depuis la base
        );
    }
}
