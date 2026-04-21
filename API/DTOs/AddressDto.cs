using System.ComponentModel.DataAnnotations;

namespace API.DTOs;

public class AddressDto
{
    [Required(ErrorMessage = "La rue est obligatoire")]
    public string Line1 { get; set; } = string.Empty;

    public string? Line2 { get; set; }

    [Required(ErrorMessage = "La ville est obligatoire")]
    public string City { get; set; } = string.Empty;

    [Required(ErrorMessage = "La région est obligatoire")]
    public string State { get; set; } = string.Empty;

    [Required(ErrorMessage = "Le code postal est obligatoire")]
    public string PostalCode { get; set; } = string.Empty;

    [Required(ErrorMessage = "Le pays est obligatoire")]
    public string Country { get; set; } = string.Empty;
}
