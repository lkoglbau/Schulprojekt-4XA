using System.ComponentModel.DataAnnotations;

public class CheckoutViewModel
{
    [Required(ErrorMessage = "Adresse ist erforderlich")]
    public string Address { get; set; }

    [Required(ErrorMessage = "Stadt ist erforderlich")]
    public string City { get; set; }

    [Required(ErrorMessage = "PLZ ist erforderlich")]
    [RegularExpression(@"^\d{4,5}$", ErrorMessage = "Ungültige PLZ")]
    public string PostalCode { get; set; }

    [Required(ErrorMessage = "Land ist erforderlich")]
    public string Country { get; set; }

    [Required(ErrorMessage = "Bitte Zahlungsmethode wählen")]
    public string PaymentMethod { get; set; }

    // Kreditkartenfelder
    [Required(ErrorMessage = "Kartennummer ist erforderlich")]
    [CreditCard(ErrorMessage = "Ungültige Kartennummer")]
    [StringLength(16, MinimumLength = 13, ErrorMessage = "Kartennummer muss zwischen 13 und 16 Ziffern haben")]
    public string CardNumber { get; set; }

    [Required(ErrorMessage = "Ablaufdatum ist erforderlich")]
    [RegularExpression(@"^(0[1-9]|1[0-2])\/\d{2}$", ErrorMessage = "Format MM/YY")]
    public string ExpiryDate { get; set; }

    [Required(ErrorMessage = "CVV ist erforderlich")]
    [RegularExpression(@"^\d{3,4}$", ErrorMessage = "Ungültiger CVV")]
    public string CVV { get; set; }

    [Required(ErrorMessage = "Name des Karteninhabers ist erforderlich")]
    public string CardHolder { get; set; }
}