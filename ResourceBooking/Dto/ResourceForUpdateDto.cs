namespace ResourceBooking.Dtos
{
    public class ResourceForUpdateDto
    {
        public int ResourceId { get; set; }
        public string Name { get; set; } = string.Empty;
        public int ResourceTypeId { get; set; }
    }
}
