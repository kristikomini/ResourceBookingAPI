using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ResourceBooking.Models;

namespace ResourceBooking.Repositories
{
    public interface IResourceRepository
    {
        Task<IEnumerable<Resource>> GetResourcesAsync();
        Task<Resource?> GetResourceByIdAsync(int resourceId);
        Task<Resource> CreateResourceAsync(Resource resource);
        Task<Resource> UpdateResourceAsync(Resource resource);
        Task<bool> DeleteResourceAsync(int resourceId);
        Task<IEnumerable<Resource>> GetAvailableResourcesAsync(
            DateTime startDate,
            DateTime endDate,
            int? resourceId = null
        );
    }
}
