using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ResourceBooking.Data;
using ResourceBooking.Models;

namespace ResourceBooking.Repositories
{
    public class ResourceRepository : IResourceRepository
    {
        private readonly DataContext _context;

        public ResourceRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Resource>> GetResourcesAsync()
        {
            try
            {
                return await _context.Resources.Include(r => r.ResourceType).ToListAsync();
            }
            catch (Exception ex)
            {
                // Log exception
                throw new Exception("Error fetching resources.", ex);
            }
        }

        public async Task<Resource?> GetResourceByIdAsync(int resourceId)
        {
            try
            {
                return await _context
                    .Resources.Include(r => r.ResourceType)
                    .FirstOrDefaultAsync(r => r.ResourceId == resourceId);
            }
            catch (Exception ex)
            {
                // Log exception
                throw new Exception("Error fetching resource by ID.", ex);
            }
        }

        public async Task<Resource> CreateResourceAsync(Resource resource)
        {
            try
            {
                _context.Resources.Add(resource);
                await _context.SaveChangesAsync();

                // Ensure ResourceType is loaded
                await _context.Entry(resource).Reference(r => r.ResourceType).LoadAsync();

                return resource;
            }
            catch (Exception ex)
            {
                // Log exception
                throw new Exception("Error creating resource.", ex);
            }
        }

        public async Task<Resource> UpdateResourceAsync(Resource resource)
        {
            try
            {
                _context.Resources.Update(resource);
                await _context.SaveChangesAsync();
                return resource;
            }
            catch (Exception ex)
            {
                // Log exception
                throw new Exception("Error updating resource.", ex);
            }
        }

        public async Task<bool> DeleteResourceAsync(int resourceId)
        {
            try
            {
                var resource = await _context.Resources.FindAsync(resourceId);
                if (resource == null)
                {
                    return false;
                }

                _context.Resources.Remove(resource);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                // Log exception
                throw new Exception("Error deleting resource.", ex);
            }
        }

        public async Task<IEnumerable<Resource>> GetAvailableResourcesAsync(
            DateTime startDate,
            DateTime endDate,
            int? resourceId = null
        )
        {
            try
            {
                var query = _context.Resources.Include(r => r.Bookings).AsQueryable();

                if (resourceId.HasValue)
                {
                    query = query.Where(r => r.ResourceId == resourceId);
                }

                return await query
                    .Where(r =>
                        !r.Bookings.Any(b => b.StartDate < endDate && b.EndDate > startDate)
                    )
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                // Log exception
                throw new Exception("Error fetching available resources.", ex);
            }
        }
    }
}
