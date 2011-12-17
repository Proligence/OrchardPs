using System;
using Orchard.Environment.Configuration;

namespace Orchard.PowerShell.Sites.Items {
    [Serializable]
    public class OrchardSite {
        public string Name { get; set; }
        public TenantState.State State { get; set; }
        public string DataConnectionString { get; set; }
        public string DataProvider { get; set; }
        public string DataTablePrefix { get; set; }
        public string EncryptionAlgorithm { get; set; }
        public string EncryptionKey { get; set; }
        public string HashAlgorithm { get; set; }
        public string HashKey { get; set; }
        public string RequestUrlHost { get; set; }
        public string RequestUrlPrefix { get; set; }
    }
}