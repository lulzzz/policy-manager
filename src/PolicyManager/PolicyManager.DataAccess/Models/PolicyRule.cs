using Microsoft.WindowsAzure.Storage.Table;
using System;

namespace PolicyManager.DataAccess.Models
{
    public class PolicyRule
        : TableEntity
    {
        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public DateTime ModifiedDate { get; set; }

        public string Category { get; set; }

        public string DisplayName { get; set; }

        public string Description { get; set; }

        public string Rule { get; set; }
    }
}
