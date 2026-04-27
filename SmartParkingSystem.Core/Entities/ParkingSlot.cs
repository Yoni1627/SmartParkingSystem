namespace SmartParkingSystem.Core.Entities
{
    public enum SlotStatus
    {
        Free,
        Booked,
        Maintenance
    }

    public class ParkingSlot
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public int ParkingZoneId { get; set; }
        public ParkingZone? ParkingZone { get; set; }

        public SlotStatus Status { get; set; } = SlotStatus.Free;

        public bool IsActive { get; set; } = true;
    }
}
