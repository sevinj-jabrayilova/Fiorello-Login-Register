using Fiorello_MVC.Data;
using Fiorello_MVC.Models;
using Fiorello_MVC.Services.Interfaces;
using Fiorello_MVC.ViewModels.Products;
using Fiorello_MVC.ViewModels.Sliders;
using Microsoft.EntityFrameworkCore;

namespace Fiorello_MVC.Services
{
    public class ProductService : IProductService
    {
        private readonly AppDbContext _context;
        private readonly IFileService _fileService;
        public ProductService(AppDbContext context,
                              IFileService fileService)
        {
            _context = context;
            _fileService = fileService;
        }

        public async Task CreateAsync(ProductCreateVM model)
        {
            List<ProductImage> images = [];

            foreach (var item in model.Images)
            {
                string fileName = _fileService.GenerateUniqueName(item.FileName);
                string path = _fileService.GeneratePath("assets/img", fileName);
                await _fileService.UploadAsync(item, path);

                images.Add(new ProductImage { Image = fileName });
            }

            images.FirstOrDefault().IsMain = true;

            Product product = new()
            {
                Name = model.Name,
                Description = model.Description,
                Price = model.Price,
                CategoryId = model.CategoryId,
                ProductImages = images
            };

            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var result = await _context.Products.FindAsync(id);

            if (result is null) return;

            _context.Products.Remove(result);
            await _context.SaveChangesAsync();
        }

        public async Task<ProductDetailVM?> DetailAsync(int? id)
        {
            if (id is null) return null;

            var dbProduct = await _context.Products
                                          .Where(m => m.Id == id)
                                          .Select(m => new ProductDetailVM
                                          {
                                              Name = m.Name,
                                              Description = m.Description,
                                              Price = m.Price,
                                              CategoryId = m.CategoryId,
                                              Images = m.ProductImages.Select(m => new ProductImageDetailVM
                                              {
                                                  IsMain = m.IsMain,
                                                  Image = m.Image,

                                              }).ToList()
                                          }).FirstOrDefaultAsync();

            return dbProduct;
        }

        public async Task EditAsync(int id, ProductEditVM model)
        {
            var dbProduct = await _context.Products.Include(p => p.ProductImages)
                                                   .FirstOrDefaultAsync(p => p.Id == id);

            if (dbProduct is null) return;

            if (model.NewImage != null)
            {
                var mainImage = dbProduct.ProductImages.FirstOrDefault(m => m.IsMain);

                if (mainImage != null)
                {
                    string oldPath = _fileService.GeneratePath("assets/img", mainImage.Image);
                    _fileService.Delete(oldPath);

                    string fileName = _fileService.GenerateUniqueName(model.NewImage.FileName);
                    string newPath = _fileService.GeneratePath("assets/img", fileName);
                    await _fileService.UploadAsync(model.NewImage, newPath);

                    mainImage.Image = fileName;
                }
                else
                {
                    string fileName = _fileService.GenerateUniqueName(model.NewImage.FileName);
                    string newPath = _fileService.GeneratePath("assets/img", fileName);
                    await _fileService.UploadAsync(model.NewImage, newPath);

                    dbProduct.ProductImages.Add(new ProductImage
                    {
                        Image = fileName,
                        IsMain = true
                    });

                    var newImage = new ProductImage
                    {
                        Image = fileName,
                        IsMain = true
                    };

                    dbProduct.ProductImages.Add(newImage);

                    
                }
            }

          await  IsMainImage(id);

            dbProduct.Name = model.Name;
            dbProduct.Description = model.Description;
            dbProduct.Price = model.Price;
            dbProduct.CategoryId = model.CategoryId;

            await _context.SaveChangesAsync();

        }

        public async Task<IEnumerable<ProductVM>> GetAllAdminAsync()
        {
            return await _context.Products.Include(m => m.Category)
                                          .Include(m => m.ProductImages)
                                          .Select(m => new ProductVM
                                          {
                                              Id = m.Id,
                                              Name = m.Name,
                                              Category = m.Category.Name,
                                              Image = m.ProductImages.FirstOrDefault(m => m.IsMain).Image,
                                          }).ToListAsync();
        }

        public async Task<IEnumerable<ProductUIVM>> GetAllAsync()
        {
            return await _context.Products.Include(m => m.ProductImages).Select(m => new ProductUIVM
            {
                Id = m.Id,
                Name = m.Name,
                Price = m.Price,
                CategoryId = m.CategoryId,
                Image = m.ProductImages.FirstOrDefault(m => m.IsMain).Image
            }).ToListAsync();
        }

        public async Task<ProductEditVM?> GetByIdAsync(int id)
        {
            var dbProduct = await _context.Products.Include(m => m.ProductImages)
                                                   .FirstOrDefaultAsync(m => m.Id == id);

            if (dbProduct == null) return null;

            return new ProductEditVM
            {
                Name = dbProduct.Name,
                Description = dbProduct.Description,
                Price = dbProduct.Price,
                CategoryId = dbProduct.CategoryId,
                ExistImage = dbProduct.ProductImages.FirstOrDefault(m => m.IsMain)?.Image
            };
        }

        public async Task<decimal> GetPriceByIdAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            return product.Price;
        }

        //edit, create hisselerde main image-ni deyismek ucun idi
        public async Task IsMainImage(int? id)
        {
            //if (id is null) return;

            //var data = await _context.ProductImages.FindAsync(id);

            //if(data is null) return;

            //var images = await _context.ProductImages.ToListAsync();
            //foreach (var item in images)
            //{
            //    item.IsMain = false;
            //}

            //data.IsMain = true;
            //_context.Update(data);
            //await _context.SaveChangesAsync();

            var selectedImage = await _context.ProductImages
                                     .Include(i => i.Product)
                                     .FirstOrDefaultAsync(i => i.Id == id);

            if (selectedImage == null) return;

            var productImages = await _context.ProductImages
                                              .Where(i => i.ProductId == selectedImage.ProductId)
                                              .ToListAsync();

            foreach (var image in productImages)
            {
                image.IsMain = false;
            }

            selectedImage.IsMain = true;

            await _context.SaveChangesAsync();
        }
    }
}
