using System.Collections.Generic;

namespace LightBlue.Standalone
{
    public class StandaloneMetadataStore
    {
        public string ContentType { get; set; }
        public Dictionary<string, string> Metadata { get; set; }
    }
}