using System;
using System.Collections.Generic;
using System.Text;

namespace PolicyManager.DataAccess.Models
{
    public class PolicyResult
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public PolicyEvaluation Result { get; set; }
    }

    public enum PolicyEvaluation
    {
        Deny,
        Allow,
    }
}
