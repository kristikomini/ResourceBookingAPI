using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ResourceBooking.Exceptions
{
    public class ResourceAlreadyBookedException : Exception
    {
        public ResourceAlreadyBookedException()
            : base("The resource is already booked during the specified time interval.") { }
    }
}
