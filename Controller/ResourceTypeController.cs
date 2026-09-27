using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ResourceBooking.Dto;
using ResourceBooking.Dtos;
using ResourceBooking.Interfaces;
using ResourceBooking.Models;
using ResourceBooking.Repositories;
using Swashbuckle.AspNetCore.Annotations;

namespace ResourceBooking.Controllers
{
    [Route("api/resourcetypes")]
    [ApiController]
    public class ResourceTypeController : ControllerBase
    {
        private readonly IResourceTypeRepository _resourceTypeRepository;

        public ResourceTypeController(IResourceTypeRepository resourceTypeRepository)
        {
            _resourceTypeRepository = resourceTypeRepository;
        }

        [HttpGet]
        [SwaggerOperation(Summary = "List of Resource Types")]
        public async Task<ActionResult<IEnumerable<ResourceTypeDto>>> GetResourceTypes()
        {
            try
            {
                var resourceTypes = await _resourceTypeRepository.GetResourceTypesAsync();
                var resourceTypeDtos = resourceTypes
                    .Select(rt => new ResourceTypeDto
                    {
                        ResourceTypeId = rt.ResourceTypeId,
                        TypeName = rt.TypeName,
                    })
                    .ToList();

                return Ok(resourceTypeDtos);
            }
            catch (Exception ex)
            {
                // Log exception
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Get Resource Type by ID")]
        public async Task<ActionResult<ResourceTypeDto>> GetResourceType(int id)
        {
            try
            {
                var resourceType = await _resourceTypeRepository.GetResourceTypeByIdAsync(id);

                if (resourceType == null)
                {
                    return NotFound("Resource Type not found.");
                }

                var resourceTypeDto = new ResourceTypeDto
                {
                    ResourceTypeId = resourceType.ResourceTypeId,
                    TypeName = resourceType.TypeName,
                };

                return Ok(resourceTypeDto);
            }
            catch (Exception ex)
            {
                // Log exception
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost]
        [SwaggerOperation(Summary = "Add Resource Type")]
        public async Task<ActionResult<ResourceTypeDto>> AddResourceType(
            ResourceTypeForCreationDto resourceTypeForCreationDto
        )
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var resourceType = new ResourceType
                {
                    TypeName = resourceTypeForCreationDto.TypeName,
                };

                var createdResourceType = await _resourceTypeRepository.CreateResourceTypeAsync(
                    resourceType
                );

                var resourceTypeDto = new ResourceTypeDto
                {
                    ResourceTypeId = createdResourceType.ResourceTypeId,
                    TypeName = createdResourceType.TypeName,
                };

                return CreatedAtAction(
                    nameof(GetResourceType),
                    new { id = createdResourceType.ResourceTypeId },
                    resourceTypeDto
                );
            }
            catch (Exception ex)
            {
                // Log exception
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut]
        [SwaggerOperation(Summary = "Update Resource Type")]
        public async Task<ActionResult<ResourceTypeDto>> UpdateResourceType(
            ResourceTypeForUpdateDto resourceTypeForUpdateDto
        )
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var resourceType = await _resourceTypeRepository.GetResourceTypeByIdAsync(
                    resourceTypeForUpdateDto.ResourceTypeId
                );

                if (resourceType == null)
                {
                    return NotFound("Resource Type not found.");
                }

                resourceType.TypeName = resourceTypeForUpdateDto.TypeName;

                var updatedResourceType = await _resourceTypeRepository.UpdateResourceTypeAsync(
                    resourceType
                );

                var resourceTypeDto = new ResourceTypeDto
                {
                    ResourceTypeId = updatedResourceType.ResourceTypeId,
                    TypeName = updatedResourceType.TypeName,
                };

                return Ok(resourceTypeDto);
            }
            catch (Exception ex)
            {
                // Log exception
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        [SwaggerOperation(Summary = "Delete Resource Type")]
        public async Task<IActionResult> DeleteResourceType(int id)
        {
            try
            {
                var success = await _resourceTypeRepository.DeleteResourceTypeAsync(id);

                if (!success)
                {
                    return NotFound("Resource Type not found.");
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
