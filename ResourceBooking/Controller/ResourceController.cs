using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ResourceBooking.Dtos;
using ResourceBooking.Models;
using ResourceBooking.Repositories;
using Swashbuckle.AspNetCore.Annotations;

namespace ResourceBooking.Controllers
{
    [Authorize]
    [Route("api/resources")]
    [ApiController]
    public class ResourceController : ControllerBase
    {
        private readonly IResourceRepository _resourceRepository;
        private readonly IMapper _mapper;

        public ResourceController(IResourceRepository resourceRepository, IMapper mapper)
        {
            _resourceRepository = resourceRepository;
            _mapper = mapper;
        }

        [HttpGet]
        [SwaggerOperation(Summary = "List of Resources")]
        public async Task<ActionResult<IEnumerable<ResourceDto>>> GetResources()
        {
            var resources = await _resourceRepository.GetResourcesAsync();
            var resourceDtos = _mapper.Map<List<ResourceDto>>(resources);

            return Ok(resourceDtos);
        }

        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Get Resource by ID")]
        public async Task<ActionResult<ResourceDto>> GetResource(int id)
        {
            var resource = await _resourceRepository.GetResourceByIdAsync(id);

            if (resource == null)
            {
                return NotFound("Resource not found.");
            }

            var resourceDto = _mapper.Map<ResourceDto>(resource);

            return Ok(resourceDto);
        }

        [HttpPost]
        [SwaggerOperation(Summary = "Add Resource")]
        public async Task<ActionResult<ResourceDto>> AddResource(
            ResourceForCreationDto resourceForCreationDto
        )
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var resource = new Resource
            {
                Name = resourceForCreationDto.Name,
                ResourceTypeId = resourceForCreationDto.ResourceTypeId,
            };

            var createdResource = await _resourceRepository.CreateResourceAsync(resource);
            var resourceDto = _mapper.Map<ResourceDto>(createdResource);

            return CreatedAtAction(
                nameof(GetResource),
                new { id = createdResource.ResourceId },
                resourceDto
            );
        }

        [HttpPut]
        [SwaggerOperation(Summary = "Update Resource")]
        public async Task<ActionResult<ResourceDto>> UpdateResource(
            ResourceForUpdateDto resourceForUpdateDto
        )
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var resource = await _resourceRepository.GetResourceByIdAsync(
                resourceForUpdateDto.ResourceId
            );

            if (resource == null)
            {
                return NotFound("Resource not found.");
            }

            resource.Name = resourceForUpdateDto.Name;
            resource.ResourceTypeId = resourceForUpdateDto.ResourceTypeId;

            var updatedResource = await _resourceRepository.UpdateResourceAsync(resource);
            var resourceDto = _mapper.Map<ResourceDto>(updatedResource);

            return Ok(resourceDto);
        }

        [HttpDelete("{id}")]
        [SwaggerOperation(Summary = "Delete Resource")]
        public async Task<IActionResult> DeleteResource(int id)
        {
            var success = await _resourceRepository.DeleteResourceAsync(id);

            if (!success)
            {
                return NotFound("Resource not found.");
            }

            return NoContent();
        }
    }
}
