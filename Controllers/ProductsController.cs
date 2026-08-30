using InventoryManagement.Models;
using InventoryManagement.Services.Interfaces;
using InventoryManagement.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagement.Controllers
{
    // Admin and Manager can fully manage products; Staff has no access.
    [Authorize(Roles = "Admin,Manager")]
    public class ProductsController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;

        public ProductsController(IProductService productService, ICategoryService categoryService)
        {
            _productService  = productService;
            _categoryService = categoryService;
        }

        // GET: Products
        public async Task<IActionResult> Index()
        {
            var products = await _productService.GetAllProductsAsync();
            var vms = products.Select(p => new ProductViewModel
            {
                Id         = p.Id,
                Name       = p.Name,
                Price      = p.Price,
                CostPrice  = p.CostPrice,
                Quantity   = p.Quantity,
                CategoryId = p.CategoryId
            }).ToList();
            return View(vms);
        }

        // GET: Products/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.Categories = await _categoryService.GetAllCategoriesAsync() ?? new List<Category>();
            return View(new ProductViewModel());
        }

        // POST: Products/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Categories = await _categoryService.GetAllCategoriesAsync();
                return View(model);
            }

            var product = new Product
            {
                Name       = model.Name,
                Price      = model.Price,
                CostPrice  = model.CostPrice,
                Quantity   = model.Quantity,
                CategoryId = model.CategoryId
            };

            await _productService.AddProductAsync(product);
            return RedirectToAction(nameof(Index));
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null) return NotFound();

            ViewBag.Categories = await _categoryService.GetAllCategoriesAsync();
            return View(new ProductViewModel
            {
                Id         = product.Id,
                Name       = product.Name,
                Price      = product.Price,
                CostPrice  = product.CostPrice,
                Quantity   = product.Quantity,
                CategoryId = product.CategoryId
            });
        }

        // POST: Products/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ProductViewModel model)
        {
            if (id != model.Id) return BadRequest();
            if (!ModelState.IsValid)
            {
                ViewBag.Categories = await _categoryService.GetAllCategoriesAsync();
                return View(model);
            }

            await _productService.UpdateProductAsync(new Product
            {
                Id         = model.Id,
                Name       = model.Name,
                Price      = model.Price,
                CostPrice  = model.CostPrice,
                Quantity   = model.Quantity,
                CategoryId = model.CategoryId
            });
            return RedirectToAction(nameof(Index));
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null) return NotFound();
            return View(new ProductViewModel
            {
                Id         = product.Id,
                Name       = product.Name,
                Price      = product.Price,
                CostPrice  = product.CostPrice,
                Quantity   = product.Quantity,
                CategoryId = product.CategoryId
            });
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _productService.DeleteProductAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
