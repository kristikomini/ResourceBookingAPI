namespace ResourceBooking.Dtos
{
    public class ResourceDto
    {
        public int ResourceId { get; set; }
        public string Name { get; set; } = string.Empty;
        public int ResourceTypeId { get; set; }
        public string ResourceTypeName { get; set; } = string.Empty;
    }
}
