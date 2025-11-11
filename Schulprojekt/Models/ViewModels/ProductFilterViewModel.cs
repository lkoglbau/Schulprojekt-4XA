using Schulprojekt.Models;

public class ProductFilterViewModel
{
    public List<Category> Categories { get; set; }
    public string SelectedCategory { get; set; }
    public List<Product> Products { get; set; }
}