using Microsoft.Extensions.Configuration;
using System;

namespace PolicyManager.DataAccess.Models
{
    public class DocumentSettings
    {
        public DocumentSettings()
        {
            if (Environment.GetEnvironmentVariables().Contains("DocumentEndpoint"))
            {
                DocumentEndpoint = new Uri(Environment.GetEnvironmentVariable("DocumentEndpoint"));
                DocumentKey = Environment.GetEnvironmentVariable("DocumentKey");
                DatabaseId = Environment.GetEnvironmentVariable("DatabaseId");
            }
        }

        public DocumentSettings(IConfigurationRoot configurationRoot)
        {
            DocumentEndpoint = new Uri(configurationRoot["DocumentEndpoint"]);
            DocumentKey = configurationRoot["DocumentKey"];
            DatabaseId = configurationRoot["DatabaseId"];
        }

        public Uri DocumentEndpoint { get; set; }

        public string DocumentKey { get; set; }

        public string DatabaseId { get; set; }
    }
}
