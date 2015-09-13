using System;
using System.Collections.Generic;

namespace Proligence.PowerShell.Provider.Console {
    [Serializable]
    public class CompletionData {
        public IList<string> Results { get; set; }
        public int CurrentMatchIndex { get; set; }
        public int ReplacementIndex { get; set; }
        public int ReplacementLength { get; set; }
    }
}