using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ResourceBooking.Dtos;
using ResourceBooking.Models;
using ResourceBooking.Services;
using Swashbuckle.AspNetCore.Annotations;

namespace ResourceBooking.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly TokenService _tokenService;
        private readonly IMapper _mapper;

        public UsersController(
            IUserRepository userRepository,
            TokenService tokenService,
            IMapper mapper
        )
        {
            _userRepository = userRepository;
            _tokenService = tokenService;
            _mapper = mapper;
        }

        [HttpGet]
        [SwaggerOperation(Summary = "Get all Users")]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetAllUsers()
        {
            var users = await _userRepository.GetUsersAsync();
            var userDtos = _mapper.Map<List<UserDto>>(users);

            return Ok(userDtos);
        }

        [AllowAnonymous]
        [HttpPost]
        [SwaggerOperation(Summary = "Register a new User")]
        public async Task<ActionResult<UserDto>> CreateUser(UserForCreationDto userForCreationDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingUsers = await _userRepository.GetUsersAsync();
            if (
                existingUsers.Any(u =>
                    string.Equals(u.Email, userForCreationDto.Email, StringComparison.OrdinalIgnoreCase)
                )
            )
            {
                return Conflict("Email already exists.");
            }

            var user = new User
            {
                Email = userForCreationDto.Email,
                Name = userForCreationDto.Name,
                LastName = userForCreationDto.LastName,
                Password = userForCreationDto.Password, // Hashing is handled by the repository.
            };

            var createdUser = await _userRepository.CreateUserAsync(user);
            var token = _tokenService.GenerateToken(createdUser);
            var userDto = _mapper.Map<UserDto>(createdUser);

            return CreatedAtAction(
                nameof(GetUserById),
                new { userId = createdUser.UserId },
                new { user = userDto, token }
            );
        }

        [HttpGet("{userId}")]
        [SwaggerOperation(Summary = "Get User by Id")]
        public async Task<ActionResult<UserDto>> GetUserById(int userId)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);

            if (user == null)
            {
                return NotFound();
            }

            var userDto = _mapper.Map<UserDto>(user);

            return Ok(userDto);
        }

        [HttpPut("{userId}")]
        [SwaggerOperation(Summary = "Update User")]
        public async Task<IActionResult> UpdateUser(int userId, UserForUpdateDto userForUpdateDto)
        {
            if (userId != userForUpdateDto.UserId)
            {
                return BadRequest("User ID mismatch");
            }

            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            user.Email = userForUpdateDto.Email;
            user.Name = userForUpdateDto.Name;
            user.LastName = userForUpdateDto.LastName;
            user.Password = userForUpdateDto.Password; // Hashing is handled by the repository.

            await _userRepository.UpdateUserAsync(user);

            return NoContent();
        }

        [HttpDelete("{userId}")]
        [SwaggerOperation(Summary = "Delete User by Id")]
        public async Task<IActionResult> DeleteUser(int userId)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            await _userRepository.DeleteUserAsync(user);
            return NoContent();
        }
    }
}
