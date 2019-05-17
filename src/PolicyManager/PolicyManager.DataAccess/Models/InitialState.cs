using Microsoft.Graph;
using System.Collections.Generic;
using System.Security.Claims;

namespace PolicyManager.DataAccess.Models
{
    public class InitialState
    {
        public ClaimsPrincipal ClaimsPrincipal { get; set; }

        public IEnumerable<Group> Groups { get; set; }
    }
}
