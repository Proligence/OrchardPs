namespace Proligence.PowerShell.Provider.Console 
{
    using System.Collections.Generic;

    public class CompletionData 
    {
        public IEnumerable<string> Results { get; set; }
        public int CurrentMatchIndex { get; set; }
        public int ReplacementIndex { get; set; }
        public int ReplacementLength { get; set; }
    }
}