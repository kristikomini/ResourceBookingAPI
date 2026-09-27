using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ResourceBooking.Dtos;
using ResourceBooking.Exceptions;
using ResourceBooking.Interfaces;
using ResourceBooking.Models;
using Swashbuckle.AspNetCore.Annotations;

namespace ResourceBooking.Controllers
{
    [Authorize]
    [Route("api/bookings")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly IMapper _mapper;

        public BookingController(IBookingRepository bookingRepository, IMapper mapper)
        {
            _bookingRepository = bookingRepository;
            _mapper = mapper;
        }

        [HttpGet]
        [SwaggerOperation(Summary = "List of Bookings")]
        public async Task<ActionResult<IEnumerable<BookingDto>>> GetBookings()
        {
            var bookings = await _bookingRepository.GetBookingsAsync();
            var bookingDtos = _mapper.Map<List<BookingDto>>(bookings);

            return Ok(bookingDtos);
        }

        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Get Booking by ID")]
        public async Task<ActionResult<BookingDto>> GetBooking(int id)
        {
            var booking = await _bookingRepository.GetBookingByIdAsync(id);

            if (booking == null)
            {
                return NotFound("Booking not found.");
            }

            var bookingDto = _mapper.Map<BookingDto>(booking);

            return Ok(bookingDto);
        }

        [HttpPost]
        [SwaggerOperation(Summary = "Add Booking")]
        public async Task<ActionResult<BookingDto>> AddBooking(BookingForCreationDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (dto.DataInizio >= dto.DataFine)
            {
                return BadRequest("DataInizio must be earlier than DataFine.");
            }

            try
            {
                var booking = _mapper.Map<Booking>(dto);
                var created = await _bookingRepository.CreateBookingAsync(booking);
                var bookingDto = _mapper.Map<BookingDto>(created);
                return CreatedAtAction(
                    nameof(GetBooking),
                    new { id = created.BookingId },
                    bookingDto
                );
            }
            catch (ResourceAlreadyBookedException ex)
            {
                return Conflict(ex.Message); // 409 is the correct status for this
            }
        }

        [HttpPut]
        [SwaggerOperation(Summary = "Update Booking")]
        public async Task<ActionResult<BookingDto>> UpdateBooking(BookingDto bookingForUpdateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (bookingForUpdateDto.DataInizio >= bookingForUpdateDto.DataFine)
            {
                return BadRequest("DataInizio must be earlier than DataFine.");
            }

            try
            {
                var booking = _mapper.Map<Booking>(bookingForUpdateDto);
                var updatedBooking = await _bookingRepository.UpdateBookingAsync(booking);

                if (updatedBooking == null)
                {
                    return NotFound("Booking not found.");
                }

                var bookingDto = _mapper.Map<BookingDto>(updatedBooking);

                return Ok(bookingDto);
            }
            catch (ResourceAlreadyBookedException ex)
            {
                return Conflict(ex.Message); // 409 is the correct status for this
            }
        }

        [HttpDelete("{id}")]
        [SwaggerOperation(Summary = "Delete Booking")]
        public async Task<IActionResult> DeleteBooking(int id)
        {
            var success = await _bookingRepository.DeleteBookingAsync(id);

            if (!success)
            {
                return NotFound("Booking not found.");
            }

            return NoContent();
        }

        [HttpGet("availability")]
        [SwaggerOperation(Summary = "Search Availability with Pagination")]
        public async Task<ActionResult<PaginatedResult<ResourceDto>>> SearchAvailability(
            [FromQuery] AvailabilitySearchDto searchDto
        )
        {
            // Validate date range
            if (searchDto.DataInizio >= searchDto.DataFine)
            {
                return BadRequest("DataInizio must be earlier than DataFine.");
            }

            var availableResources = await _bookingRepository.GetAvailableResourcesAsync(
                searchDto.DataInizio,
                searchDto.DataFine,
                searchDto.CodiceRisorsa,
                searchDto.Page,
                searchDto.PageSize
            );

            var resourceDtos = _mapper.Map<PaginatedResult<ResourceDto>>(availableResources);

            return Ok(resourceDtos);
        }
    }
}
