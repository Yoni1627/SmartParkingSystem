using System;

namespace SmartParkingSystem.Core.Entities
{
    public enum ReservationStatus
    {
        Pending,
        Confirmed,
        Cancelled,
        Completed
    }

    public class Reservation
    {
        public int Id { get; set; }

        public string UserId { get; set; } = string.Empty;
        
        public int ParkingSlotId { get; set; }
        public ParkingSlot? ParkingSlot { get; set; }

        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        public decimal TotalAmount { get; set; }

        public ReservationStatus Status { get; set; } = ReservationStatus.Pending;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
