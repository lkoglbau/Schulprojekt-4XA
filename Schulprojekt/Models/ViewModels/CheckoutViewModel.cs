using System.ComponentModel.DataAnnotations;

public class CheckoutViewModel
{
    [Required]
    public string Address { get; set; }

    [Required]
    public string City { get; set; }

    [Required]
    public string PostalCode { get; set; }

    [Required]
    public string Country { get; set; }

    [Required]
    public string PaymentMethod { get; set; }
}


