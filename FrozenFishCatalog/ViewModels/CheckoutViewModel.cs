using System.ComponentModel.DataAnnotations;

namespace FrozenFishCatalog.ViewModels;

public class CheckoutViewModel
{
    public CartViewModel Cart { get; set; } = new();

    [Required]
    [Display(Name = "Shipping Address")]
    public string ShippingAddress { get; set; } = string.Empty;

    [Required]
    public string City { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Postal Code")]
    public string PostalCode { get; set; } = string.Empty;

    [Required]
    public string Country { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Card Number")]
    [CreditCard]
    public string CardNumber { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Cardholder Name")]
    public string CardholderName { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Expiry Date")]
    [RegularExpression(@"^(0[1-9]|1[0-2])\/([0-9]{2})$", ErrorMessage = "Invalid expiry date format. Use MM/YY")]
    public string ExpiryDate { get; set; } = string.Empty;

    [Required]
    [Display(Name = "CVV")]
    [RegularExpression(@"^[0-9]{3,4}$", ErrorMessage = "Invalid CVV")]
    public string Cvv { get; set; } = string.Empty;
}
