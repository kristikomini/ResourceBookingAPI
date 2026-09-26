using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ResourceBooking.Dto;
using ResourceBooking.Dtos;
using ResourceBooking.Models;
using ResourceBooking.Repositories;
using Swashbuckle.AspNetCore.Annotations;

namespace ResourceBooking.Controllers
{
    [Route("api/resources")]
    [ApiController]
    public class ResourceController : ControllerBase
    {
        private readonly IResourceRepository _resourceRepository;
        private readonly IMapper _mapper;

        // Ensure there is only one constructor
        public ResourceController(IResourceRepository resourceRepository, IMapper mapper)
        {
            _resourceRepository = resourceRepository;
            _mapper = mapper;
        }

        [HttpGet]
        [SwaggerOperation(Summary = "List of Resources")]
        public async Task<ActionResult<IEnumerable<ResourceDto>>> GetResources()
        {
            try
            {
                var resources = await _resourceRepository.GetResourcesAsync();
                var resourceDtos = _mapper.Map<List<ResourceDto>>(resources);

                return Ok(resourceDtos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Get Resource by ID")]
        public async Task<ActionResult<ResourceDto>> GetResource(int id)
        {
            try
            {
                var resource = await _resourceRepository.GetResourceByIdAsync(id);

                if (resource == null)
                {
                    return NotFound("Resource not found.");
                }

                var resourceDto = new ResourceDto
                {
                    ResourceId = resource.ResourceId,
                    Name = resource.Name,
                    ResourceTypeName = resource.ResourceType.TypeName,
                };

                return Ok(resourceDto);
            }
            catch (Exception ex)
            {
                // Log exception
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
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

            try
            {
                var resource = new Resource
                {
                    Name = resourceForCreationDto.Name,
                    ResourceTypeId = resourceForCreationDto.ResourceTypeId,
                };

                var createdResource = await _resourceRepository.CreateResourceAsync(resource);

                var resourceDto = new ResourceDto
                {
                    ResourceId = createdResource.ResourceId,
                    Name = createdResource.Name,
                    ResourceTypeName = createdResource.ResourceType?.TypeName ?? string.Empty,
                };

                return CreatedAtAction(
                    nameof(GetResource),
                    new { id = createdResource.ResourceId },
                    resourceDto
                );
            }
            catch (Exception ex)
            {
                // Log exception
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
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

            try
            {
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

                var resourceDto = new ResourceDto
                {
                    ResourceId = updatedResource.ResourceId,
                    Name = updatedResource.Name,
                    ResourceTypeName = updatedResource.ResourceType.TypeName,
                };

                return Ok(resourceDto);
            }
            catch (Exception ex)
            {
                // Log exception
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        [SwaggerOperation(Summary = "Delete Resource")]
        public async Task<IActionResult> DeleteResource(int id)
        {
            try
            {
                var success = await _resourceRepository.DeleteResourceAsync(id);

                if (!success)
                {
                    return NotFound("Resource not found.");
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                // Log exception
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
