using BikeThefts.Api.DTO;
using BikeThefts.Api.Mapper;
using BikeThefts.Api.Settings;
using BikeThefts.DataAccess;
using BikeThefts.DataAccess.Interfaces;
using BikeThefts.DataAccess.Settings;
using BikeThefts.Domain;
using BikeThefts.Domain.Entities;
using BikeThefts.Domain.Interfaces;
using IdentityModel.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Polly;
using Polly.Extensions.Http;
using System;
using System.IO;
using System.Net.Http;
using System.Reflection;

namespace BikeThefts.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            BikeIndexSettings bikeSettings = new();
            Configuration.Bind("BikeIndex", bikeSettings);
            services.Configure<CacheSettings>(Configuration.GetSection("Cache"));
            services.Configure<Domain.Settings.Locations>(Configuration.GetSection("Locations"));
            services.AddAutoMapper(typeof(OrganizationProfile));
            services.AddControllers().AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());
                options.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
            });

            services.AddSwaggerGenNewtonsoftSupport();

            services.AddScoped<IBikeIndexService, BikeIndexService>();
            services.AddScoped<IBikeTheftsDomain, BikeTheftsDomain>();
            services.AddScoped<ICacheService, CacheService>();
            services.AddLogging();


            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "BikeThefts.Api", Version = "v1" });
                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });

            services.AddHttpClient("bikeindex", c =>
            {
                c.BaseAddress = new Uri(bikeSettings.BaseUrl);

            }).AddClientAccessTokenHandler()
             .SetHandlerLifetime(TimeSpan.FromMinutes(5))  //Set lifetime to five minutes
             .AddPolicyHandler(GetRetryPolicy());

            services.AddAccessTokenManagement(options =>
            {
                options.Client.Clients.Add("bikeindex", new ClientCredentialsTokenRequest
                {
                    Address = bikeSettings.TokenUrl,
                    ClientId = bikeSettings.ClientId,
                    ClientSecret = bikeSettings.Secret
                });
            });
            services.AddMemoryCache();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "BikeThefts.Api v1"));
                app.UseExceptionHandler("/error-local-development");
            }
            else
            {
                app.UseExceptionHandler("/error");
            }

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        private IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound)
                .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
        }
    }
}
