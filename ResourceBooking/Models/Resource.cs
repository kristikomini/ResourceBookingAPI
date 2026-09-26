using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ResourceBooking.Models
{
    [Table("ResourceInfo")]
    public class Resource
    {
        [Key]
        public int ResourceId { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public int ResourceTypeId { get; set; }

        [ForeignKey("ResourceTypeId")]
        public ResourceType ResourceType { get; set; } = null!;

        //Navigation property
        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    }
}
