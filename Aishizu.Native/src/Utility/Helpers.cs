using System.Text.Json.Serialization;
using System.Text.Json;

namespace Aishizu.Native
{
    public static class ObjExt
    {
        public static string ToJson(this object obj)
        {
            JsonSerializerOptions options = new()
            {
                WriteIndented = true,
                IncludeFields = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };
            return JsonSerializer.Serialize(obj, options);
        }
    }
}
