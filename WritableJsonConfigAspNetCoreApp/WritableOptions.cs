using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace WritableJsonConfigAspNetCoreApp
{
    public class WritableOptions<T> : IWritableOptions<T> where T : class, new()
    {
        private readonly ConfigurationRoot _configurationRoot;
        private readonly IWebHostEnvironment _environment;
        private readonly bool _forceReloadAfterWrite;
        private readonly IOptionsMonitor<T> _options;
        private readonly string _section;
        private readonly string _settingsFile;

        public WritableOptions(IWebHostEnvironment environment,
            IOptionsMonitor<T> options,
            ConfigurationRoot configurationRoot,
            string section,
            string settingsFile,
            bool forceReloadAfterWrite = false)
        {
            _environment = environment;
            _options = options;
            _configurationRoot = configurationRoot;
            _section = section;
            _settingsFile = settingsFile;
            _forceReloadAfterWrite = forceReloadAfterWrite;
        }

        public T Get(string name)
        {
            return _options.Get(name);
        }

        public async Task Update(Action<T> applyChanges)
        {
            var fullPath = Path.IsPathRooted(_settingsFile)
                ? _settingsFile
                : _environment.ContentRootFileProvider.GetFileInfo(_settingsFile).PhysicalPath;

            ExpandoObject config;
            applyChanges(Value);

            if (!File.Exists(fullPath))
            {
                config = new ExpandoObject();
                ((IDictionary<string, object>)config).Add(_section, Value);
            }
            else
            {
                config = JsonSerializer.Deserialize<ExpandoObject>(File.ReadAllText(fullPath));
                ((IDictionary<string, object>)config)[_section] = Value;
            }

            File.WriteAllText(fullPath, JsonSerializer.Serialize(config, new JsonSerializerOptions {WriteIndented = true}));

            if (_forceReloadAfterWrite) _configurationRoot.Reload();
        }

        public T Value => _options.CurrentValue;
    }
}
