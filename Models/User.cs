using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ResourceBooking.Models
{
    [Table("UserInfo")]
    public class User
    {
        [Key]
        public int UserId { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string LastName { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;

        //Navigation property one - to - many
        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    }
}
