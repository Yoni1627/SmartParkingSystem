using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SmartParkingSystem.Core.Entities
{
    public class ParkingZone
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(200)]
        public string Location { get; set; } = string.Empty;

        [Range(0, 1000)]
        public decimal HourlyRate { get; set; }

        public virtual ICollection<ParkingSlot>? Slots { get; set; } = new List<ParkingSlot>();
    }
}
