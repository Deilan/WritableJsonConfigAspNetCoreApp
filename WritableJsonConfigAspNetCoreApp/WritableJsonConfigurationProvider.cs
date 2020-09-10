using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;

namespace WritableJsonConfigAspNetCoreApp
{
    /// <summary>
    /// A JSON file based <see cref="FileConfigurationProvider"/>.
    /// </summary>
    public class WritableJsonConfigurationProvider : JsonConfigurationProvider
    {
        private readonly JsonSerializerOptions _options;

        /// <summary>
        /// Initializes a new instance with the specified source.
        /// </summary>
        /// <param name="source">The source settings.</param>
        public WritableJsonConfigurationProvider(JsonConfigurationSource source) : base(source)
        {
        }
        /// <summary>
        /// Initializes a new instance with the specified source.
        /// </summary>
        /// <param name="source">The source settings.</param>
        public WritableJsonConfigurationProvider(JsonConfigurationSource source, JsonSerializerOptions options) : base(source)
        {
            _options = options;
        }

        public override void Set(string key, string value)
        {
            base.Set(key, value);
            var seed = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            var result = Data.Aggregate(seed, Func);
            File.WriteAllText(Source.Path, JsonSerializer.Serialize(result, _options));
        }

        private Dictionary<string, object> Func(Dictionary<string, object> map, KeyValuePair<string, string> pair)
        {
            var parts = pair.Key.Split(ConfigurationPath.KeyDelimiter);
            var context = map;
            for (var i = 0; i < parts.Length - 1; i++)
            {
                if (context.ContainsKey(parts[i]))
                {
                    context = (Dictionary<string, object>)context[parts[i]];
                }
                else
                {
                    var dictionary = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
                    context[parts[i]] = dictionary;
                    context = dictionary;
                }
            }

            object result = null;
            if (bool.TryParse(pair.Value, out var booleanResult))
            {
                result = booleanResult;
            }
            else if (decimal.TryParse(pair.Value, out var decimalResult))
            {
                result = decimalResult;
            }
            else if (StringComparer.OrdinalIgnoreCase.Equals(null, pair.Value))
            {
                result = null;
            }
            else
            {
                result = pair.Value;
            }

            context[parts[^1]] = result;
            return map;
        }

        private static Dictionary<string, object> Unparse(string key, string value)
        {
            var result = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            var queue = new Queue<string>(key.Split(ConfigurationPath.KeyDelimiter));
            var context = result;
            while (queue.Any())
            {
                var next = queue.Dequeue();
                if (queue.Any())
                {
                    context[next] = context = new Dictionary<string, object>();
                }
                else
                {
                    context[next] = value;
                }
            }

            return result;
        }
    }
}
