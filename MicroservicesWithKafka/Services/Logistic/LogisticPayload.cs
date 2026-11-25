using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.Json;

namespace MicroservicesWithKafka.Services.Logistic
{
    public static class LogisticPayload
    {
        public static JObject Parse(object? payload)
        {
            if (payload == null)
                throw new ArgumentException("Payload is required");

            return payload is JsonElement element
                ? JObject.Parse(element.GetRawText())
                : JObject.FromObject(payload);
        }

        public static string RequireType(JObject json, params string[] allowed)
        {
            var type = Str(json, "type");
            if (string.IsNullOrWhiteSpace(type))
                throw new ArgumentException("type is required");
            if (!allowed.Contains(type, StringComparer.Ordinal))
                throw new ArgumentException("type must be one of " + string.Join(", ", allowed) + ".");
            return type;
        }

        public static string Str(JObject json, string key)
        {
            var token = json.GetValue(key, StringComparison.OrdinalIgnoreCase);
            return token == null || token.Type == JTokenType.Null ? string.Empty : (token.ToString().Trim());
        }

        public static string RequireStr(JObject json, string key)
        {
            var value = Str(json, key);
            if (value.Length == 0)
                throw new ArgumentException($"{key} is required");
            return value;
        }

        public static int? Int(JObject json, string key)
        {
            var text = Str(json, key);
            if (text.Length == 0) return null;
            if (int.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out var value)) return value;
            throw new ArgumentException($"{key} must be a number");
        }

        public static int RequireInt(JObject json, string key)
            => Int(json, key) ?? throw new ArgumentException($"{key} is required");

        public static JArray DataArray(JObject json)
            => json.GetValue("data", StringComparison.OrdinalIgnoreCase) as JArray ?? new JArray();
    }

    public static class LogisticResponse
    {
        public static object Build(int statusCode, string statusMsg, object? data)
        {
            int count = data switch
            {
                null => 0,
                string => 1,
                IEnumerable list => list.Cast<object?>().Count(),
                _ => 1
            };

            return new { StatusCode = statusCode, StatusMsg = statusMsg, Count = count, Data = data };
        }
    }
}
