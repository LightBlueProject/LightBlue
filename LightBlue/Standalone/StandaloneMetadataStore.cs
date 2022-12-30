using Azure.Storage.Blobs.Models;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace LightBlue.Standalone
{
    internal class StandaloneMetadataStore
    {
        public string ContentType { get; set; }
        public Dictionary<string, string> Metadata { get; set; }

        public void WriteToStreamAndClose(FileStream fileStream)
        {
            using (fileStream)
            {
                var serialized = JsonSerializer.SerializeToUtf8Bytes(this);
                fileStream.Write(serialized, 0, serialized.Length);
            }
        }

        public static StandaloneMetadataStore ReadFromStream(FileStream fileStream)
        {
            using (var reader = new StreamReader(fileStream, System.Text.Encoding.UTF8, true, 1024, true))
            {
                var content = reader.ReadToEnd();
                return JsonSerializer.Deserialize<StandaloneMetadataStore>(content);
            }
        }
    }
}