using System.Collections.Generic;

namespace PolicyManager.DataAccess.Models
{
    public class Policy
    {
        public string Resource { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public IEnumerable<string> Groups { get; set; }

        public IEnumerable<string> Attributes { get; set; }

        public string Target { get; set; }

        public IEnumerable<string> Actions { get; set; }

        public IEnumerable<string> Expressions { get; set; }
    }
}
