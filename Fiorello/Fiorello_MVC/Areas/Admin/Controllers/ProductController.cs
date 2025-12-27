using Fiorello_MVC.Services;
using Fiorello_MVC.Services.Interfaces;
using Fiorello_MVC.ViewModels.Products;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Fiorello_MVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        public ProductController(IProductService productService,
                                 ICategoryService categoryService)
        {
            _productService = productService;
            _categoryService = categoryService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return View(await _productService.GetAllAdminAsync());
        }

        //create

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var categories = await _categoryService.GetAllAdminAsync();
            ViewBag.categories = categories.Select(m => new SelectListItem { Value = m.Id.ToString(), Text = m.Name });
            return View(); ;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductCreateVM request)
        {
            if (!ModelState.IsValid)
            {
                var categories = await _categoryService.GetAllAdminAsync();
                ViewBag.categories = categories.Select(m => new SelectListItem { Value = m.Id.ToString(), Text = m.Name });
                return View(request);
            }

            await _productService.CreateAsync(request);

            return RedirectToAction(nameof(Index));
        }

        //detail

        [HttpGet]
        public async Task<IActionResult> Detail(int id)
        {
            var product = await _productService.DetailAsync(id);

            if (product is null) return NotFound();

            return View(product);
        }

        //delete

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await _productService.DeleteAsync(id);
            return Ok();
        }

        //edit

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id is null) return BadRequest();

            var dbProduct = await _productService.GetByIdAsync(id.Value);
            if (dbProduct == null) return NotFound();

            var categories = await _categoryService.GetAllAdminAsync();
            ViewBag.categories = categories.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Name
            });

            return View(dbProduct);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id, ProductEditVM request)
        {
            if (id is null) return BadRequest();

            if (!ModelState.IsValid)
            {
                var categories = await _categoryService.GetAllAdminAsync();
                ViewBag.categories = categories.Select(m => new SelectListItem
                {
                    Value = m.Id.ToString(),
                    Text = m.Name
                });

                return View(request);
            }

            await _productService.EditAsync(id.Value, request);

            return RedirectToAction(nameof(Index));
        }

    }
}
