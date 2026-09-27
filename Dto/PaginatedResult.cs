namespace ResourceBooking.Dtos
{
    public class PaginatedResult<T>
    {
        public int TotalResults { get; set; }
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public List<T> Results { get; set; } = new();
    }
}
