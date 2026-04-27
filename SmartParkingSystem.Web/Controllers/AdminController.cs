using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartParkingSystem.Core.Entities;
using SmartParkingSystem.Infrastructure.Data;

namespace SmartParkingSystem.Web.Controllers
{
    // [Authorize(Roles = "Admin")] // Commented out for dev simplicity until seed data is ready
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        // --- ZONES ---
        public async Task<IActionResult> Zones()
        {
            var zones = await _context.ParkingZones.ToListAsync();
            return View(zones);
        }

        [HttpPost]
        public async Task<IActionResult> CreateZone([FromForm] ParkingZone zone)
        {
            if (ModelState.IsValid)
            {
                _context.Add(zone);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Zones));
            }
            return RedirectToAction(nameof(Zones));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteZone(int id)
        {
            var zone = await _context.ParkingZones.FindAsync(id);
            if (zone != null)
            {
                _context.ParkingZones.Remove(zone);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Zones));
        }

        // --- SLOTS ---
        public async Task<IActionResult> Slots(int? zoneId)
        {
            ViewBag.Zones = await _context.ParkingZones.ToListAsync();
            var query = _context.ParkingSlots.Include(s => s.ParkingZone).AsQueryable();
            
            if (zoneId.HasValue && zoneId.Value > 0)
            {
                query = query.Where(s => s.ParkingZoneId == zoneId.Value);
                ViewBag.SelectedZone = zoneId.Value;
            }

            return View(await query.ToListAsync());
        }

        [HttpPost]
        public async Task<IActionResult> CreateSlot([FromForm] ParkingSlot slot)
        {
            if (ModelState.IsValid)
            {
                _context.Add(slot);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Slots), new { zoneId = slot.ParkingZoneId });
            }
            return RedirectToAction(nameof(Slots));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteSlot(int id)
        {
            var slot = await _context.ParkingSlots.FindAsync(id);
            if (slot != null)
            {
                _context.ParkingSlots.Remove(slot);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Slots));
        }

        [HttpPost]
        public async Task<IActionResult> UpdateStatus(int id, SlotStatus status)
        {
            var slot = await _context.ParkingSlots.FindAsync(id);
            if (slot != null)
            {
                slot.Status = status;
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Slots));
        }
    }
}
