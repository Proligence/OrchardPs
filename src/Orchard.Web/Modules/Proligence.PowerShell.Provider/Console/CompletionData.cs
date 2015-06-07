namespace Proligence.PowerShell.Provider.Console 
{
    using System;
    using System.Collections.Generic;

    [Serializable]
    public class CompletionData 
    {
        public IList<string> Results { get; set; }
        public int CurrentMatchIndex { get; set; }
        public int ReplacementIndex { get; set; }
        public int ReplacementLength { get; set; }
    }
}