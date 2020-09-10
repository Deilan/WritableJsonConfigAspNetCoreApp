using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;

namespace WritableJsonConfigAspNetCoreApp
{
    /// <summary>
    /// Represents a JSON file as an <see cref="IConfigurationSource"/>.
    /// </summary>
    public class WritableJsonConfigurationSource : JsonConfigurationSource
    {
        public WritableJsonConfigurationSource(JsonSerializerOptions options)
        {
            Options = options;
        }

        public WritableJsonConfigurationSource() : this(null) { }

        public JsonSerializerOptions Options { get; set; }

        /// <summary>
        /// Builds the <see cref="JsonConfigurationProvider"/> for this source.
        /// </summary>
        /// <param name="builder">The <see cref="IConfigurationBuilder"/>.</param>
        /// <returns>A <see cref="JsonConfigurationProvider"/></returns>
        public override IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            EnsureDefaults(builder);
            return new WritableJsonConfigurationProvider(this, Options);
        }
    }
}
