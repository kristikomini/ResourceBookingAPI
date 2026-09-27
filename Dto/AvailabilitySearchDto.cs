namespace ResourceBooking.Dtos
{
    public class AvailabilitySearchDto
    {
        public DateTime DataInizio { get; set; }
        public DateTime DataFine { get; set; }
        public int? CodiceRisorsa { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
