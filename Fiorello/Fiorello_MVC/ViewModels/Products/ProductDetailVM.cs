using Fiorello_MVC.Models;

namespace Fiorello_MVC.ViewModels.Products
{
    public class ProductDetailVM
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int CategoryId { get; set; }
        public ICollection<ProductImageDetailVM> Images { get; set; }
    }
}
