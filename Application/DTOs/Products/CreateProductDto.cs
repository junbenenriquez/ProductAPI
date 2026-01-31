using System.ComponentModel.DataAnnotations;

namespace Application;

public class CreateProductDto
{
    [Required(ErrorMessage = "Product name is required")]
    [StringLength(30, MinimumLength = 6, ErrorMessage = "Product name must be 6–30 characters")]
    public string Name { get; set; } = default!;
    public string? Description { get; set; }

    [Required(ErrorMessage = "Price is required")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than zero")]
    public decimal Price { get; set; }
    public int Stock { get; set; }
}
