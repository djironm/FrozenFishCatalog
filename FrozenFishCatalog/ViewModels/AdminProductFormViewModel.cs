using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FrozenFishCatalog.ViewModels;

public class AdminProductFormViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre es requerido")]
    [Display(Name = "Nombre")]
    public string Name { get; set; } = string.Empty;

    [Display(Name = "Descripción")]
    public string Description { get; set; } = string.Empty;

    [Display(Name = "URL de Imagen")]
    public string ImageUrl { get; set; } = string.Empty;

    [Display(Name = "Condiciones de Envío")]
    public string ShippingTerms { get; set; } = string.Empty;

    [Required(ErrorMessage = "La categoría es requerida")]
    [Display(Name = "Categoría")]
    public int CategoryId { get; set; }

    public List<SelectListItem> Categories { get; set; } = new();

    // Weight variant IDs (0 = not yet created)
    public int Weight1kgId { get; set; }
    public int Weight2kgId { get; set; }
    public int Weight3kgId { get; set; }

    [Required, Range(0.01, 99999, ErrorMessage = "Precio inválido")]
    [Display(Name = "Precio 1 kg")]
    public decimal Price1kg { get; set; }

    [Required, Range(0, 99999, ErrorMessage = "Stock inválido")]
    [Display(Name = "Stock 1 kg")]
    public int Stock1kg { get; set; }

    [Required, Range(0.01, 99999, ErrorMessage = "Precio inválido")]
    [Display(Name = "Precio 2 kg")]
    public decimal Price2kg { get; set; }

    [Required, Range(0, 99999, ErrorMessage = "Stock inválido")]
    [Display(Name = "Stock 2 kg")]
    public int Stock2kg { get; set; }

    [Required, Range(0.01, 99999, ErrorMessage = "Precio inválido")]
    [Display(Name = "Precio 3 kg")]
    public decimal Price3kg { get; set; }

    [Required, Range(0, 99999, ErrorMessage = "Stock inválido")]
    [Display(Name = "Stock 3 kg")]
    public int Stock3kg { get; set; }
}
