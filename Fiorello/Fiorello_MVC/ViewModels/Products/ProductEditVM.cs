using System.ComponentModel.DataAnnotations;

namespace Fiorello_MVC.ViewModels.Products
{
    public class ProductEditVM
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public decimal Price { get; set; }
        public int CategoryId { get; set; }
        public string? ExistImage { get; set; }
        public IFormFile? NewImage { get; set; }
    }
}
