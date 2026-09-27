namespace ResourceBooking.Dtos
{
    public class BookingForCreationDto
    {
        public int ResourceId { get; set; }
        public int UserId { get; set; }
        public DateTime DataInizio { get; set; }
        public DateTime DataFine { get; set; }
    }
}
