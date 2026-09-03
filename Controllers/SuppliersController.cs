using InventoryManagement.Models;
using InventoryManagement.Services.Interfaces;
using InventoryManagement.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagement.Controllers
{
    [Authorize(Roles = "Admin,Manager")]
    public class SuppliersController : Controller
    {
        private readonly ISupplierService _supplierService;

        public SuppliersController(ISupplierService supplierService)
        {
            _supplierService = supplierService;
        }

        // GET: /Suppliers
        public async Task<IActionResult> Index()
        {
            var suppliers = await _supplierService.GetAllAsync();
            var vms = suppliers.Select(s => new SupplierViewModel
            {
                Id                  = s.Id,
                Name                = s.Name,
                ContactPerson       = s.ContactPerson,
                Phone               = s.Phone,
                Email               = s.Email,
                City                = s.City,
                Country             = s.Country,
                IsActive            = s.IsActive,
                PurchaseOrderCount  = s.PurchaseOrders?.Count ?? 0
            }).ToList();

            if (TempData["Success"] is string ok)  ViewBag.Success = ok;
            if (TempData["Error"]   is string err) ViewBag.Error   = err;
            return View(vms);
        }

        // GET: /Suppliers/Create
        public IActionResult Create() => View(new SupplierViewModel());

        // POST: /Suppliers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SupplierViewModel vm)
        {
            if (!ModelState.IsValid) return View(vm);

            await _supplierService.AddAsync(new Supplier
            {
                Name          = vm.Name,
                ContactPerson = vm.ContactPerson,
                Phone         = vm.Phone,
                Email         = vm.Email,
                Address       = vm.Address,
                City          = vm.City,
                Country       = vm.Country,
                IsActive      = vm.IsActive
            });

            TempData["Success"] = $"Supplier '{vm.Name}' created.";
            return RedirectToAction(nameof(Index));
        }

        // GET: /Suppliers/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var s = await _supplierService.GetByIdAsync(id);
            if (s == null) return NotFound();

            return View(new SupplierViewModel
            {
                Id            = s.Id,
                Name          = s.Name,
                ContactPerson = s.ContactPerson,
                Phone         = s.Phone,
                Email         = s.Email,
                Address       = s.Address,
                City          = s.City,
                Country       = s.Country,
                IsActive      = s.IsActive
            });
        }

        // POST: /Suppliers/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, SupplierViewModel vm)
        {
            if (id != vm.Id) return BadRequest();
            if (!ModelState.IsValid) return View(vm);

            await _supplierService.UpdateAsync(new Supplier
            {
                Id            = vm.Id,
                Name          = vm.Name,
                ContactPerson = vm.ContactPerson,
                Phone         = vm.Phone,
                Email         = vm.Email,
                Address       = vm.Address,
                City          = vm.City,
                Country       = vm.Country,
                IsActive      = vm.IsActive
            });

            TempData["Success"] = $"Supplier '{vm.Name}' updated.";
            return RedirectToAction(nameof(Index));
        }

        // GET: /Suppliers/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var s = await _supplierService.GetByIdAsync(id);
            if (s == null) return NotFound();

            return View(new SupplierViewModel
            {
                Id                  = s.Id,
                Name                = s.Name,
                ContactPerson       = s.ContactPerson,
                Phone               = s.Phone,
                Email               = s.Email,
                City                = s.City,
                Country             = s.Country,
                IsActive            = s.IsActive,
                PurchaseOrderCount  = s.PurchaseOrders?.Count ?? 0
            });
        }

        // POST: /Suppliers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var s = await _supplierService.GetByIdAsync(id);
            if (s == null) return NotFound();

            if (s.PurchaseOrders?.Any() == true)
            {
                TempData["Error"] = $"Cannot delete '{s.Name}' — it has {s.PurchaseOrders.Count} purchase order(s). Deactivate it instead.";
                return RedirectToAction(nameof(Index));
            }

            await _supplierService.DeleteAsync(id);
            TempData["Success"] = $"Supplier '{s.Name}' deleted.";
            return RedirectToAction(nameof(Index));
        }
    }
}
