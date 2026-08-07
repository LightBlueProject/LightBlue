using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

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
                WriteToStreamInternal(fileStream);
            }
        }

        public void WriteToStream(FileStream fileStream) => WriteToStreamInternal(fileStream);

        public static StandaloneMetadataStore ReadFromStream(FileStream fileStream)
        {
            using (var reader = new StreamReader(fileStream, System.Text.Encoding.UTF8, true, StandaloneAzureBlockBlob.BufferSize, true))
            {
                var content = reader.ReadToEnd();
                return JsonSerializer.Deserialize<StandaloneMetadataStore>(content);
            }
        }

        public static async Task<StandaloneMetadataStore> ReadFromStreamAsync(FileStream fileStream)
        {
            using (var reader = new StreamReader(fileStream, System.Text.Encoding.UTF8, true, StandaloneAzureBlockBlob.BufferSize, true))
            {
                var content = await reader.ReadToEndAsync();
                return JsonSerializer.Deserialize<StandaloneMetadataStore>(content);
            }
        }

        private void WriteToStreamInternal(FileStream fileStream)
        {
            var serialized = JsonSerializer.SerializeToUtf8Bytes(this);
            fileStream.Write(serialized, 0, serialized.Length);
        }
    }
}