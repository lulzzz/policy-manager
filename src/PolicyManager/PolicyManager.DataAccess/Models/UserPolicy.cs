using Microsoft.WindowsAzure.Storage.Table;
using System;

namespace PolicyManager.DataAccess.Models
{
    public class UserPolicy
        : TableEntity
    {
        public string PolicyId { get; set; }

        public string PolicyCategory { get; set; }

        public string UserPrincipalName { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public DateTime ModifiedDate { get; set; }
    }
}
