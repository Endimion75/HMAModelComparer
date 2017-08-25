using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace CompareAPI
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddCors(o => o.AddPolicy("MyCrossPolicy", builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

            //Also look at the Web.config
            var increaseFactor = 2;
            services.Configure<FormOptions>(options =>
            {
                options.ValueLengthLimit = options.ValueLengthLimit * increaseFactor;
                options.MultipartBodyLengthLimit = options.MultipartBodyLengthLimit * increaseFactor;
                options.MultipartHeadersLengthLimit = options.MultipartHeadersLengthLimit * increaseFactor;
            });

            //This controls how the json serializer names the field names E.g. Id => id;
            //.AddJsonOptions(o=>o.SerializerSettings.ContractResolver = new DefaultContractResolver());
            //If you want to remove it just use:
            //services.AddMvc();
            services.AddMvc().AddJsonOptions(o => o.SerializerSettings.ContractResolver = new DefaultContractResolver());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseMvc();
        }
    }
}
