using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace WritableJsonConfigAspNetCoreApp
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        //public Startup(IHostEnvironment env)
        //{
        //    Configuration = new ConfigurationBuilder()
        //        .SetBasePath(env.ContentRootPath)
        //        .AddWritableJsonFile("appsettings.json")
        //        .Build();
        //}

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IConfigurationRoot>(provider =>
            {
                return (IConfigurationRoot)provider.GetRequiredService<IConfiguration>();
            });
            services.AddControllers();
            //services.Configure<MySettings>(Configuration.GetSection("MySettings"));
            //services.AddSingleton<CalculatorService>();
            //services.AddWritableOptions();
            //var name = "";
            //var config = Configuration;
            services.ConfigureWritable2<MySettings>(Configuration.GetSection("MySettings"));

            //services.ConfigureOptions<MySettings>()
            //services.AddSingleton<IConfigureOptions<TOptions>>(new ConfigureNamedOptions<TOptions>(name, configureOptions));
            //services.AddOptions();
            //var src = new ConfigurationChangeTokenSource<MySettings>(name, config);
            //var opt = new NamedConfigureFromConfigurationOptions<MySettings>(name, config, configureBinder);
            //services.AddSingleton<IOptionsChangeTokenSource<MySettings>>(src);
            //services.AddSingleton<IConfigureOptions<MySettings>>(opt);
            //services.Configure<MySettings>(Configuration);
            //var w = services.ConfigureOptions<ConfigureMySettingsOptions>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }

    public class MySettings
    {
        public string MyOption { get; set; }
        public Nested MyObject { get; set; }

        public class Nested
        {
            public string MyDeepOption { get; set; }
        }
    }

    public static class WritableOptionsServiceCollectionExtensions
    {
        public static IServiceCollection AddWritableOptions(this IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.AddOptions();
            services.TryAdd(ServiceDescriptor.Singleton(typeof(IWritableOptions2<>), typeof(WritableOptionsManager<>)));
            //services.TryAdd(ServiceDescriptor.Singleton(typeof(IOptions<>), typeof(OptionsManager<>)));
            //services.TryAdd(ServiceDescriptor.Scoped(typeof(IOptionsSnapshot<>), typeof(OptionsManager<>)));
            //services.TryAdd(ServiceDescriptor.Singleton(typeof(IOptionsMonitor<>), typeof(OptionsMonitor<>)));
            //services.TryAdd(ServiceDescriptor.Transient(typeof(IOptionsFactory<>), typeof(OptionsFactory<>)));
            //services.TryAdd(ServiceDescriptor.Singleton(typeof(IOptionsMonitorCache<>), typeof(OptionsCache<>)));
            return services;
        }
    }

    public class ConfigureMySettingsOptions : IConfigureOptions<MySettings>, IPostConfigureOptions<MySettings>
    {
        private readonly CalculatorService _calculator;
        public ConfigureMySettingsOptions(CalculatorService calculator)
        {
            _calculator = calculator;
        }

        public void Configure(MySettings options)
        {
            //options.MyOption = _calculator.DoComplexCalcaultion();
        }

        public void PostConfigure(string name, MySettings options)
        {

        }
    }

    public static class ServiceCollectionExtensions2
    {
        /// <param name="forceReloadAfterWrite">If you have some problem with config file reloading</param>
        public static IServiceCollection ConfigureWritable2<TOptions>(this IServiceCollection services, IConfigurationSection config)
            where TOptions : class, new()
        {
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
            var name = Options.DefaultName;
            Action<BinderOptions> configureBinder = _ => { };
            services.AddWritableOptions();
            services.AddSingleton<IOptionsChangeTokenSource<TOptions>>(new ConfigurationChangeTokenSource<TOptions>(name, config));
            services.AddSingleton<IConfigureOptions<TOptions>>(new NamedConfigureFromConfigurationOptions<TOptions>(name, config, configureBinder));
            services.AddScoped<IWritableOptions2<TOptions>>(provider =>
            {
                var factory = provider.GetRequiredService<IOptionsFactory<TOptions>>();
                //var configurationRoot = provider.GetRequiredService<ConfigurationRoot>();
                var configurationRoot = (ConfigurationRoot)provider.GetRequiredService<IConfiguration>();
                return new WritableOptionsManager<TOptions>(factory, configurationRoot, config);
            });

            return services;
        }
    }

    public class CalculatorService
    {
        public string DoComplexCalcaultion()
        {
            return "trololo";
        }
    }
}
