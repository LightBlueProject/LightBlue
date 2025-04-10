using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace LightBlue.MultiHost.Configuration
{
    // Allows for the use of either the enum name or the integer value in JSON serialization/deserialization
    public class IconOptionConverter : JsonConverter<IconOption>
    {
        public override IconOption Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Number)
            {
                return (IconOption)reader.GetInt32();
            }

            if (reader.TokenType == JsonTokenType.String)
            {
                var value = reader.GetString();
                if (Enum.TryParse(value, true, out IconOption result))
                {
                    return result;
                }
            }

            return default;
        }

        public override void Write(Utf8JsonWriter writer, IconOption value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }
    }
}
