using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SharedModule.Converters;

public class DateOnlyJsonConverter : JsonConverter<DateOnly>
{
    protected virtual string DateFormat => "yyyy-MM-dd";

    public override DateOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String && reader.GetString() is string dateString && !string.IsNullOrEmpty(dateString))
        {
            return DateOnly.ParseExact(dateString, DateFormat, CultureInfo.InvariantCulture);
        }
        throw new JsonException($"Invalid date format. Expected format: {DateFormat}");
    }

    public override void Write(Utf8JsonWriter writer, DateOnly value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString(DateFormat, CultureInfo.InvariantCulture));
    }
}

public class DateOnlyJsonConverterType2 : DateOnlyJsonConverter
{
    protected override string DateFormat => "yyyy/MM/dd";
}