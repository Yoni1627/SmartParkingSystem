using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using SmartParkingSystem.Core.Entities;
using SmartParkingSystem.Infrastructure.Data;
using SmartParkingSystem.Web.Hubs;
using System.Security.Claims;

namespace SmartParkingSystem.Web.Controllers
{
    public class BookingController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IHubContext<ParkingHub> _hubContext;

        public BookingController(ApplicationDbContext context, IHubContext<ParkingHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }

        public async Task<IActionResult> Create(int slotId)
        {
            var slot = await _context.ParkingSlots
                .Include(s => s.ParkingZone)
                .FirstOrDefaultAsync(s => s.Id == slotId);

            if (slot == null || slot.Status != SlotStatus.Free)
            {
                TempData["Error"] = "This slot is no longer available.";
                return RedirectToAction("Index", "Home");
            }

            var reservation = new Reservation
            {
                ParkingSlotId = slotId,
                ParkingSlot = slot,
                StartTime = DateTime.UtcNow.AddMinutes(15), // Default to start in 15 mins
                EndTime = DateTime.UtcNow.AddHours(2)       // Default duration 2 hours
            };

            return View(reservation);
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmBooking(Reservation model)
        {
            var slot = await _context.ParkingSlots
                .Include(s => s.ParkingZone)
                .FirstOrDefaultAsync(s => s.Id == model.ParkingSlotId);

            if (slot == null) return NotFound();

            // Validation Rules
            if (model.StartTime < DateTime.UtcNow)
            {
                ModelState.AddModelError("StartTime", "Cannot book in the past.");
            }
            if (model.StartTime > DateTime.UtcNow.AddHours(24))
            {
                ModelState.AddModelError("StartTime", "Cannot book more than 24 hours in advance.");
            }
            if ((model.EndTime - model.StartTime).TotalMinutes < 30)
            {
                ModelState.AddModelError("EndTime", "Minimum booking duration is 30 minutes.");
            }
            if ((model.EndTime - model.StartTime).TotalHours > 12)
            {
                ModelState.AddModelError("EndTime", "Maximum booking duration is 12 hours.");
            }

            if (!ModelState.IsValid)
            {
                model.ParkingSlot = slot;
                return View("Create", model);
            }

            // Calculate total amount
            var hours = (decimal)(model.EndTime - model.StartTime).TotalHours;
            model.TotalAmount = hours * slot.ParkingZone.HourlyRate;
            
            // Note: In a real app we'd get the actual UserId. Using a placeholder or User.FindFirstValue.
            model.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "guest-user";
            model.Status = ReservationStatus.Confirmed; // We skip pending for simplicity if no payment wall

            _context.Reservations.Add(model);
            
            // Update slot status
            slot.Status = SlotStatus.Booked;
            
            await _context.SaveChangesAsync();

            // Broadcast the change to all connected clients via SignalR
            await _hubContext.Clients.All.SendAsync("ReceiveSlotStatusUpdate", slot.Id, SlotStatus.Booked.ToString());

            // Redirect to Payment/Success
            return RedirectToAction("Payment", new { id = model.Id });
        }

        public async Task<IActionResult> Payment(int id)
        {
            var reservation = await _context.Reservations
                .Include(r => r.ParkingSlot)
                .ThenInclude(s => s.ParkingZone)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (reservation == null) return NotFound();

            return View(reservation);
        }

        [HttpPost]
        public async Task<IActionResult> ProcessPayment(int reservationId, PaymentMethod method)
        {
            var reservation = await _context.Reservations.FindAsync(reservationId);
            if (reservation == null) return NotFound();

            var payment = new Payment
            {
                ReservationId = reservationId,
                Amount = reservation.TotalAmount,
                Method = method,
                Status = PaymentStatus.Success,
                TransactionId = Guid.NewGuid().ToString().Substring(0, 8).ToUpper()
            };

            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();

            return RedirectToAction("Success", new { id = reservationId });
        }

        public async Task<IActionResult> Success(int id)
        {
            var reservation = await _context.Reservations
                .Include(r => r.ParkingSlot)
                .ThenInclude(s => s.ParkingZone)
                .FirstOrDefaultAsync(r => r.Id == id);

            return View(reservation);
        }
    }
}
