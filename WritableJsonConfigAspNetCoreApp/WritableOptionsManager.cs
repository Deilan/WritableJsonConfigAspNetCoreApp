using System;
using System.ComponentModel;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace WritableJsonConfigAspNetCoreApp
{
    public class WritableOptionsManager<TOptions> : OptionsManager<TOptions>, IWritableOptions2<TOptions>
        where TOptions : class, new()
    {
        private readonly IOptionsFactory<TOptions> _factory;
        private readonly IConfigurationRoot _configurationRoot;
        private readonly IConfigurationSection _section;
        private readonly OptionsCache<TOptions> _cache = new OptionsCache<TOptions>(); // Note: this is a private cache

        /// <summary>
        /// Initializes a new instance with the specified options configurations.
        /// </summary>
        /// <param name="factory">The factory to use to create options.</param>
        public WritableOptionsManager(IOptionsFactory<TOptions> factory, IConfigurationRoot configurationRoot, IConfigurationSection section) : base(factory)
        {
            _factory = factory;
            _configurationRoot = configurationRoot;
            _section = section;
        }

        public void Update(string key, string val)
        {
            var finalKey = string.Join(ConfigurationPath.KeyDelimiter, _section.Path, key);
            _configurationRoot[finalKey] = val;
        }

        public void Update(TOptions instance)
        {
            foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(instance.GetType()))
            {
                var oldValue = property.GetValue(OriginalValue);
                var newValue = property.GetValue(instance);
                if (!oldValue.Equals(newValue))
                {
                    var finalKey = string.Join(ConfigurationPath.KeyDelimiter, _section.Path, property.Name);
                    _configurationRoot[finalKey] = newValue.ToString();
                }
            }
        }

        private TOptions OriginalValue { get; set; }

        public override TOptions Get(string name)
        {
            name ??= Options.DefaultName;

            // Store the options in our instance cache
            return _cache.GetOrAdd(name, () =>
            {
                var result = _factory.Create(name);
                OriginalValue = Clone(result);
                return result;
            });
        }

        private static TOptions Clone(TOptions instance)
        {
            MethodInfo MemberwiseCloneMethod =
                typeof(Object).GetMethod("MemberwiseClone", BindingFlags.Instance | BindingFlags.NonPublic);

            TOptions output = (TOptions)MemberwiseCloneMethod.Invoke(instance, null);
            return output;
        }
    }
}
