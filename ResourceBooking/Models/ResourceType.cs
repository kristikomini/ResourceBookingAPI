using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ResourceBooking.Models
{
    [Table("ResourceTypeInfo")]
    public class ResourceType
    {
        [Key]
        public int ResourceTypeId { get; set; }

        [Required]
        public string TypeName { get; set; } = string.Empty;

        public ICollection<Resource> Resources { get; set; } = new List<Resource>();
    }
}
