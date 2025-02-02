﻿using System.Buffers;
using System.Linq.Expressions;
using Microsoft.Extensions.DependencyInjection;

namespace WritableJsonConfigAspNetCoreApp
{

    public static class ServiceCollectionExtensions
    {
        /// <param name="forceReloadAfterWrite">If you have some problem with config file reloading</param>
        public static IServiceCollection ConfigureWritable<TOptions>(this IServiceCollection services,
            string sectionName = null, string settingsFile = "appsettings.json", bool forceReloadAfterWrite = false)
            where TOptions : class, new()
        {
            sectionName ??= typeof(TOptions).Name;

            services.AddOptions();
            //services.AddSingleton<IOptionsChangeTokenSource<TOptions>>(provider =>
            //{
            //    var configurationSection = provider.GetRequiredService<IConfiguration>().GetSection(sectionName);
            //    return new ConfigurationChangeTokenSource<TOptions>(typeof(TOptions).Name, configurationSection);
            //});
            //services.AddSingleton<IConfigureOptions<TOptions>>(provider =>
            //{
            //    var configurationSection = provider.GetRequiredService<IConfiguration>().GetSection(sectionName);
            //    return new NamedConfigureFromConfigurationOptions<TOptions>(sectionName, configurationSection,
            //        _ => { });
            //});
            //services.AddScoped<IWritableOptions<TOptions>>(provider =>
            //{
            //    var environment = provider.GetRequiredService<IWebHostEnvironment>();
            //    var options = provider.GetRequiredService<IOptionsMonitor<TOptions>>();
            //    var configurationRoot = (ConfigurationRoot)provider.GetRequiredService<IConfiguration>();
            //    return new WritableOptions<TOptions>(environment, options, configurationRoot, sectionName, settingsFile,
            //        forceReloadAfterWrite);
            //});

            return services;
        }
    }
}
