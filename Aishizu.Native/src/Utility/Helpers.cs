using System.Text.Json.Serialization;
using System.Text.Json;
using System.Collections.Generic;
using Aishizu.Native.Actions;
using System;

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
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                Converters = { new JsonStringEnumConverter() }
            };
            return JsonSerializer.Serialize(obj, options);
        }
    }

    public static class aszActionExt
    {
       
    }
}
