namespace Orchard.Management.PsProvider.Host {
    public class OrchardHostContext {
        public string OrchardDirectory { get; set; }
        public string VirtualPath { get; set; }
        public string WorkingDirectory { get; set; }
        public ReturnCodes StartSessionResult { get; set; }
        public ReturnCodes RetryResult { get; set; }
        public OrchardHost OrchardHost { get; set; }
    }
}