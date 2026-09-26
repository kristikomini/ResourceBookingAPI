using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ResourceBooking.Models
{
    [Table("BookingInfo")]
    public class Booking
    {
        [Key]
        public int BookingId { get; set; }

        [Required]
        public int ResourceId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        //Navigation properties one - to - many
        [ForeignKey("ResourceId")]
        public Resource Resource { get; set; } = null!;

        [ForeignKey("UserId")]
        public User User { get; set; } = null!;
    }
}
