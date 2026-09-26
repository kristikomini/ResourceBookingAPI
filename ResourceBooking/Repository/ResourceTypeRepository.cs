using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ResourceBooking.Data;
using ResourceBooking.Interfaces;
using ResourceBooking.Models;

namespace ResourceBooking.Repositories
{
    public class ResourceTypeRepository : IResourceTypeRepository
    {
        private readonly DataContext _context;

        public ResourceTypeRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ResourceType>> GetResourceTypesAsync()
        {
            try
            {
                return await _context.ResourceTypes.ToListAsync();
            }
            catch (Exception ex)
            {
                // Log exception
                throw new Exception("Error fetching resource types.", ex);
            }
        }

        public async Task<ResourceType?> GetResourceTypeByIdAsync(int resourceTypeId)
        {
            try
            {
                return await _context.ResourceTypes.FindAsync(resourceTypeId);
            }
            catch (Exception ex)
            {
                // Log exception
                throw new Exception("Error fetching resource type by ID.", ex);
            }
        }

        public async Task<ResourceType> CreateResourceTypeAsync(ResourceType resourceType)
        {
            try
            {
                _context.ResourceTypes.Add(resourceType);
                await _context.SaveChangesAsync();
                return resourceType;
            }
            catch (Exception ex)
            {
                // Log exception
                throw new Exception("Error creating resource type.", ex);
            }
        }

        public async Task<ResourceType> UpdateResourceTypeAsync(ResourceType resourceType)
        {
            try
            {
                _context.ResourceTypes.Update(resourceType);
                await _context.SaveChangesAsync();
                return resourceType;
            }
            catch (Exception ex)
            {
                // Log exception
                throw new Exception("Error updating resource type.", ex);
            }
        }

        public async Task<bool> DeleteResourceTypeAsync(int resourceTypeId)
        {
            try
            {
                var resourceType = await _context.ResourceTypes.FindAsync(resourceTypeId);
                if (resourceType == null)
                {
                    return false;
                }

                _context.ResourceTypes.Remove(resourceType);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                // Log exception
                throw new Exception("Error deleting resource type.", ex);
            }
        }
    }
}
