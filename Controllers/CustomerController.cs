using InventoryManagement.Models;
using InventoryManagement.Services.Interfaces;
using InventoryManagement.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagement.Controllers
{
    public class CustomerController : Controller
    {
        private readonly ICustomerService _customerService;
        private readonly IAreaService _areaService;

        public CustomerController(
            ICustomerService customerService,
            IAreaService areaService)
        {
            _customerService = customerService;
            _areaService = areaService;
        }

        // GET: Customer
        public async Task<IActionResult> Index()
        {
            var customers = await _customerService.GetAllCustomerAsync();

            var customerVMs = customers.Select(c => new CustomerViewModel
            {
                Id = c.Id,
                Name = c.Name,
                Email = c.Email,
                PhoneNumber = c.PhoneNumber,
                Address = c.Address,
                City = c.City,
                Country = c.Country,
                AreaId = c.AreaId
            }).ToList();

            var areas = await _areaService.GetAllAreasAsync();
            ViewBag.Areas = areas;

            return View(customerVMs);
        }

        // GET: Customer/Create
        public async Task<IActionResult> Create()
        {
            var areas = await _areaService.GetAllAreasAsync();

            ViewBag.Areas = areas;
            return View();
        }

        // POST: Customer/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CustomerViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var areas = await _areaService.GetAllAreasAsync();
                ViewBag.Areas = areas;

                return View(model);
            }

            var customer = new Customer
            {
                Name = model.Name,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                Address = model.Address,
                City = model.City,
                Country = model.Country,
                AreaId = model.AreaId
            };

            await _customerService.AddCustomerAsync(customer);

            return RedirectToAction(nameof(Index));
        }

        // GET: Customer/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var customer = await _customerService.GetCustomerByIdAsync(id);

            if (customer == null)
                return NotFound();

            var model = new CustomerViewModel
            {
                Id = customer.Id,
                Name = customer.Name,
                Email = customer.Email,
                PhoneNumber = customer.PhoneNumber,
                Address = customer.Address,
                City = customer.City,
                Country = customer.Country,
                AreaId = customer.AreaId   // pre-select the customer's current area
            };

            var areas = await _areaService.GetAllAreasAsync();
            ViewBag.Areas = areas;

            return View(model);
        }

        // POST: Customer/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            CustomerViewModel model)
        {
            if (id != model.Id)
                return BadRequest();

            if (!ModelState.IsValid)
            {
                // repopulate dropdown, otherwise it renders empty on validation failure
                var areas = await _areaService.GetAllAreasAsync();
                ViewBag.Areas = areas;

                return View(model);
            }

            var customer = new Customer
            {
                Id = model.Id,
                Name = model.Name,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                Address = model.Address,
                City = model.City,
                Country = model.Country,
                AreaId = model.AreaId
            };

            await _customerService.UpdateCustomerAsync(customer);

            return RedirectToAction(nameof(Index));
        }

        // GET: Customer/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var customer = await _customerService.GetCustomerByIdAsync(id);

            if (customer == null)
                return NotFound();

            var model = new CustomerViewModel
            {
                Id = customer.Id,
                Name = customer.Name,
                Email = customer.Email,
                PhoneNumber = customer.PhoneNumber,
                Address = customer.Address,
                City = customer.City,
                Country = customer.Country,
                AreaId = customer.AreaId
            };

            return View(model);
        }

        // POST: Customer/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _customerService.DeleteCustomerAsync(id);

            return RedirectToAction(nameof(Index));
        }

        // GET: Customer/GetCustomersByArea
        [HttpGet]
        public async Task<JsonResult> GetCustomersByArea(int areaId)
        {
            var customers =
                await _customerService.GetCustomersByAreaAsync(areaId);

            var result = customers.Select(c => new
            {
                c.Id,
                c.Name,
                c.Email,
                c.PhoneNumber,
                c.Address,
                c.City,
                c.Country
            });

            return Json(result);
        }
    }
}