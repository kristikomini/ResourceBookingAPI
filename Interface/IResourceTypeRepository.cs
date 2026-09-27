using System.Collections.Generic;
using System.Threading.Tasks;
using ResourceBooking.Models;

namespace ResourceBooking.Interfaces
{
    public interface IResourceTypeRepository
    {
        Task<IEnumerable<ResourceType>> GetResourceTypesAsync();
        Task<ResourceType?> GetResourceTypeByIdAsync(int resourceTypeId);
        Task<ResourceType> CreateResourceTypeAsync(ResourceType resourceType);
        Task<ResourceType> UpdateResourceTypeAsync(ResourceType resourceType);
        Task<bool> DeleteResourceTypeAsync(int resourceTypeId);
    }
}
