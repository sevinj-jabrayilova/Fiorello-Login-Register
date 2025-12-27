using Fiorello_MVC.Models;

namespace Fiorello_MVC.ViewModels.Products
{
    public class ProductImageDetailVM
    {
        public string Image { get; set; }
        public bool IsMain { get; set; } = false;
    }
}
