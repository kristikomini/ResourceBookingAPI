using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ResourceBooking.Dto;
using ResourceBooking.Dtos;
using ResourceBooking.Models;
using ResourceBooking.Repositories;
using ResourceBooking.Services;
using Swashbuckle.AspNetCore.Annotations;

namespace ResourceBooking.Controllers
{
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
            try
            {
                var users = await _userRepository.GetUsersAsync();
                var userDtos = _mapper.Map<List<UserDto>>(users);

                return Ok(userDtos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [AllowAnonymous]
        [HttpPost]
        [SwaggerOperation(Summary = "Add new User")]
        public async Task<ActionResult<User>> CreateUser(UserForCreationDto userForCreationDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var existingUser = await _userRepository.GetUsersAsync();
                if (existingUser.Any(u => u.Email == userForCreationDto.Email))
                {
                    return Conflict("Email already exists.");
                }

                var user = new User
                {
                    Email = userForCreationDto.Email,
                    Name = userForCreationDto.Name,
                    LastName = userForCreationDto.LastName,
                    Password = userForCreationDto.Password, // Hashing will be handled by the repository
                };

                var createdUser = await _userRepository.CreateUserAsync(user);
                var token = _tokenService.GenerateToken(createdUser);
                return CreatedAtAction(
                    nameof(GetUserById),
                    new { userId = createdUser.UserId },
                    new { createdUser, token }
                );
            }
            catch (Exception)
            {
                // Log exception
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{userId}")]
        [SwaggerOperation(Summary = "Get User by Id")]
        public async Task<ActionResult<UserDto>> GetUserById(int userId)
        {
            try
            {
                var user = await _userRepository.GetUserByIdAsync(userId);

                if (user == null)
                {
                    return NotFound();
                }

                var userDto = _mapper.Map<UserDto>(user);

                return Ok(userDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("{userId}")]
        [SwaggerOperation(Summary = "Update User")]
        public async Task<IActionResult> UpdateUser(int userId, UserForUpdateDto userForUpdateDto)
        {
            if (userId != userForUpdateDto.UserId)
            {
                return BadRequest("User ID mismatch");
            }

            try
            {
                var user = await _userRepository.GetUserByIdAsync(userId);
                if (user == null)
                {
                    return NotFound();
                }

                user.Email = userForUpdateDto.Email;
                user.Name = userForUpdateDto.Name;
                user.LastName = userForUpdateDto.LastName;
                user.Password = userForUpdateDto.Password; // Hashing will be handled by the repository

                await _userRepository.UpdateUserAsync(user);

                return NoContent();
            }
            catch (Exception)
            {
                // Log exception
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("{userId}")]
        [SwaggerOperation(Summary = "Delete User by Id")]
        public async Task<IActionResult> DeleteUser(int userId)
        {
            try
            {
                var user = await _userRepository.GetUserByIdAsync(userId);
                if (user == null)
                {
                    return NotFound();
                }

                await _userRepository.DeleteUserAsync(user);
                return NoContent();
            }
            catch (Exception)
            {
                // Log exception
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
