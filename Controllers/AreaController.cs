using InventoryManagement.Models;
using InventoryManagement.Services.Interfaces;
using InventoryManagement.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagement.Controllers
{
    public class AreaController : Controller
    {
        private readonly IAreaService _areaService;

        public AreaController(IAreaService areaService)
        {
            _areaService = areaService;
        }

        // GET: Area
        public async Task<IActionResult> Index()
        {
            var areas = await _areaService.GetAllAreasAsync();

            var areaVMs = areas.Select(a => new AreaViewModel
            {
                Id = a.AreaId,
                Name = a.AreaName
            }).ToList();

            return View(areaVMs);
        }

        // GET: Area/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Area/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AreaViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var area = new Area
            {
                AreaName = model.Name
            };

            await _areaService.AddAreaAsync(area);

            return RedirectToAction(nameof(Index));
        }

        // GET: Area/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var area = await _areaService.GetAreaByIdAsync(id);

            if (area == null)
                return NotFound();

            var model = new AreaViewModel
            {
                Id = area.AreaId,
                Name = area.AreaName
            };

            return View(model);
        }

        // POST: Area/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, AreaViewModel model)
        {
            if (id != model.Id)
                return BadRequest();

            if (!ModelState.IsValid)
                return View(model);

            var area = new Area
            {
                AreaId = model.Id,
                AreaName = model.Name
            };

            await _areaService.UpdateAreaAsync(area);

            return RedirectToAction(nameof(Index));
        }

        // GET: Area/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var area = await _areaService.GetAreaByIdAsync(id);

            if (area == null)
                return NotFound();

            var model = new AreaViewModel
            {
                Id = area.AreaId,
                Name = area.AreaName
            };

            return View(model);
        }

        // POST: Area/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _areaService.DeleteAreaAsync(id);

            return RedirectToAction(nameof(Index));
        }
    }
}