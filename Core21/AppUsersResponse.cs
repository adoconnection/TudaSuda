using System;
using System.Collections.Generic;

namespace TudaSuda
{
    public class AppUsersResponse : AppResponse
    {
        public IList<Guid> UserGuids { get; set; }
    }
}