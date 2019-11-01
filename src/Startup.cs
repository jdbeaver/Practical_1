using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
using src.Services.Abstract;
using Swashbuckle.AspNetCore.Swagger;

namespace src
{
    public class Startup
    {
        private readonly ILogger _logger;
        public Startup(IConfiguration configuration,  ILogger<Startup> logger)
        {
            Configuration = configuration;
            _logger = logger;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            
            services.AddTransient<IAWSGateway, AWSGateway>();
            //services.AddHttpClient<IAWSGateway, AWSGateway>();
            services.AddHttpClient();
            services.AddHttpClient("AWSGatewayClient", client =>
            {
                client.BaseAddress = new Uri("https://7syfvky1vd.execute-api.us-west-2.amazonaws.com");
                client.DefaultRequestHeaders.Add("Accept", "application/json");
            });
            services.AddSwaggerGen(n =>
            {
                n.SwaggerDoc("v1",
                    new Info
                    {
                        Title = "Practical API",
                        Version = "v1",
                        Description = "Code challenge API to run multiple site lookup services.",
                        Contact = new Contact()
                        {
                            Name = "John Beaver",
                            Email = "jdbeav@gmail.com",
                            Url = "https://crunchingtime.com"

                        }
                    });
                // XML Documentation
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                n.IncludeXmlComments(xmlPath);

                
            });
            //services.AddSingleton<IRdapInfo, RdapInfo>();
            //services.AddHttpClient<IRdapInfo, FreeGeoIP>();
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
                app.UseHsts();
            }

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
            c.SwaggerEndpoint("../swagger/v1/swagger.json", "Practical API v1");
            c.RoutePrefix = string.Empty; //without this defaults to example petstore swagger!!!!
            });

            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
