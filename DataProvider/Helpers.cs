namespace DataServices;

public static class Helpers
{

    public static TTargetType? Clone<TSourceType, TTargetType>(TSourceType obj)
        where TSourceType : class
        where TTargetType : class
    {
        if (obj == null)
        {
            return null;
        }

        var serialized = System.Text.Json.JsonSerializer.Serialize(obj, new System.Text.Json.JsonSerializerOptions()
        {
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
            WriteIndented = false,
            Converters = {
            new System.Text.Json.Serialization.JsonStringEnumConverter()
        },
            IncludeFields = true
        });

        return System.Text.Json.JsonSerializer.Deserialize<TTargetType>(serialized);
    }
}
