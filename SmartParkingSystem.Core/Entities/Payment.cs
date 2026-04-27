using System;

namespace SmartParkingSystem.Core.Entities
{
    public enum PaymentMethod
    {
        CreditCard,
        MobileMoney,
        Cash,
        Wallet
    }

    public enum PaymentStatus
    {
        Pending,
        Success,
        Failed,
        Refunded
    }

    public class Payment
    {
        public int Id { get; set; }

        public int ReservationId { get; set; }
        public Reservation? Reservation { get; set; }

        public decimal Amount { get; set; }

        public PaymentMethod Method { get; set; }

        public PaymentStatus Status { get; set; } = PaymentStatus.Pending;

        public DateTime PaymentDate { get; set; } = DateTime.UtcNow;

        public string? TransactionId { get; set; }
    }
}
