using System.ComponentModel.DataAnnotations;

namespace API.DTOs;

public class CreateProductDto
{
    [Required(ErrorMessage = "Le nom est obligatoire")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "La description est obligatoire")]
	public string Description { get; set; } = string.Empty;

    [Range(0.01, double.MaxValue, ErrorMessage = "Le prix doit être supérieur à 0")]
	public decimal Price { get; set; }

    [Required(ErrorMessage = "L'url de la photo est obligatoire")]
	public string PictureUrl { get; set; } = string.Empty;

    [Required(ErrorMessage = "Le type est obligatoire")]
	public string Type { get; set; } = string.Empty;

    [Required(ErrorMessage = "La marque est obligatoire")]
	public string Brand { get; set; } = string.Empty;

    [Range(1, int.MaxValue, ErrorMessage = "La quantité en stock doit être d'au moins 1")]
	public int QuantityInStock { get; set; }
}
