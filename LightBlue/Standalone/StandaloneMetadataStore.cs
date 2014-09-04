using System.Collections.Generic;
using System.IO;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LightBlue.Standalone
{
    internal class StandaloneMetadataStore
    {
        private JObject _existingJObject;
        public string ContentType { get; set; }
        public Dictionary<string, string> Metadata { get; set; }

        public void WriteToStreamAndClose(FileStream fileStream)
        {
            var @object = GetUpdatedJObject();
            using (var writer = new JsonTextWriter(new StreamWriter(fileStream)))
            {
                new JsonSerializer().Serialize(writer, @object);
            }
        }

        public static StandaloneMetadataStore ReadFromStream(FileStream fileStream)
        {
            var reader = new JsonTextReader(new StreamReader(fileStream));
            return FromJsonObject(JObject.Load(reader));
        }
        
        private static StandaloneMetadataStore FromJsonObject(JObject jObject)
        {
            return new StandaloneMetadataStore
            {
                ContentType = jObject["ContentType"].Value<string>(),
                Metadata = jObject["Metadata"].ToObject<Dictionary<string, string>>(),
                _existingJObject = jObject
            };
        }

        private JObject GetUpdatedJObject()
        {
            var updatedJObject = JObject.FromObject(this);
            if (_existingJObject == null)
            {
                return updatedJObject;
            }
            _existingJObject.Merge(updatedJObject);
            return _existingJObject;
        }
    }
}