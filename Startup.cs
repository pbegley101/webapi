using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Registry;
using StarWars.Models;
using Microsoft.Extensions.Http;
using RestEase;
using StarWars.Proxies;
using Microsoft.AspNetCore.Mvc.Formatters;

namespace StarWars
{
    public class Startup
    {
        private IPolicyRegistry<string> policyRegistry;
        readonly string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";


        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();

            if (env.IsDevelopment())
            {
                var appAssembly = Assembly.Load(new AssemblyName(env.ApplicationName));
                if (appAssembly != null)
                {
                    builder.AddUserSecrets(appAssembly, optional: true);
                }
            }

            Configuration = builder.Build();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddHealthChecks()
    
           .AddAsyncCheck("Http", async () =>
           {
               using (HttpClient client = new HttpClient())
               {
                   try
                   {
                       //var response = await client.GetAsync("http://localhost:5000");
                       var response = await client.GetAsync(Configuration["CharacterAPIOptions:BaseUrl"]);

                       if (!response.IsSuccessStatusCode)
                       {
                           throw new Exception("Url not responding with 200 OK");
                       }
                   }
                   catch (Exception)
                   {
                       return await Task.FromResult(Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Unhealthy());
                   }
               }
               return await Task.FromResult(Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Healthy());
           });


            services.AddCors(options =>
            {
                options.AddPolicy(MyAllowSpecificOrigins,
                builder =>
                {
                    builder.WithOrigins("http://localhost:3000"
                                        ).AllowAnyHeader()
                                .AllowAnyMethod(); 
                });
            });

            services.AddMvc(options =>
            {
                options.OutputFormatters.Add(new XmlSerializerOutputFormatter());
                options.RespectBrowserAcceptHeader = true; // false by default

            }).SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
                       








            // Options for particular external services
            services.Configure<CharacterAPIOptions>(Configuration.GetSection("CharacterAPIOptions"));

            services.AddSwaggerDocument(config =>
            {
                config.PostProcess = document =>
                {
                    document.Info.Version = "v1";
                    document.Info.Title = "StarWars Character API";
                    document.Info.Description = "Starwars Character API enriched with Asp.Net Core features";
                    document.Info.TermsOfService = "None";
                    document.Info.Contact = new NSwag.OpenApiContact
                    {
                        Name = "Paul Begley",
                        Email = string.Empty,
                        Url = ""
                    };
                    document.Info.License = new NSwag.OpenApiLicense
                    {
                        Name = "",
                        Url = ""
                    };
                };
            });

            ConfigurePolicies(services);
            ConfigureHttpClients(services);

        }

        private void ConfigurePolicies(IServiceCollection services)
        {
            policyRegistry = services.AddPolicyRegistry();
            var timeoutPolicy = Policy.TimeoutAsync<HttpResponseMessage>(TimeSpan.FromMilliseconds(1500));
            policyRegistry.Add("timeout", timeoutPolicy);
        }

        private void ConfigureHttpClients(IServiceCollection services)
        {
            services.AddHttpClient("ISWApi", options =>
            {
                options.BaseAddress = new Uri(Configuration["CharacterAPIOptions:BaseUrl"]);
                options.Timeout = TimeSpan.FromMilliseconds(15000);
                options.DefaultRequestHeaders.Add("ClientFactory", "Check");
            })
            .AddPolicyHandlerFromRegistry("timeout")
            .AddTransientHttpErrorPolicy(p => p.RetryAsync(3))
            .AddTypedClient(client => RestClient.For<ISWApi>(client));
        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();

            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }


            app.UseOpenApi();
            app.UseSwaggerUi3();


            app.UseCors(MyAllowSpecificOrigins);

            app.UseHealthChecks("/healthcheck");
            app.UseHttpsRedirection();
            app.UseMvcWithDefaultRoute();
        }
    }
}
