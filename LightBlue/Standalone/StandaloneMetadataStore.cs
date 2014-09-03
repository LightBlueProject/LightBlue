using System.Collections.Generic;
using System.IO;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LightBlue.Standalone
{
    internal class StandaloneMetadataStore
    {
        private JObject JObject { get; set; }
        public string ContentType { get; set; }
        public Dictionary<string, string> Metadata { get; set; }

        public static StandaloneMetadataStore FromJsonObject(JObject jObject)
        {
            return new StandaloneMetadataStore
            {
                ContentType = jObject["ContentType"].Value<string>(),
                Metadata = jObject["Metadata"].ToObject<Dictionary<string, string>>(),
                JObject = jObject
            };
        }

        public void WriteToStreamAndClose(FileStream fileStream)
        {
            var jObject = JObject ?? JObject.FromObject(this);
            jObject["ContentType"] = ContentType;
            jObject["Metadata"] = JObject.FromObject(Metadata);
            using (var writer = new JsonTextWriter(new StreamWriter(fileStream)))
            {
                new JsonSerializer().Serialize(writer, jObject);
            }
        }

        public static StandaloneMetadataStore ReadFromStream(FileStream fileStream)
        {
            var reader = new JsonTextReader(new StreamReader(fileStream));
            return FromJsonObject(JObject.Load(reader));
        }
    }
}