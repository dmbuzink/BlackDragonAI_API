using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlackDragonAIAPI.Models;
using BlackDragonAIAPI.Models.Validation;
using BlackDragonAIAPI.StorageHandlers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Pomelo.EntityFrameworkCore.MySql.Storage.Internal;

namespace BlackDragonAIAPI
{
    public class Startup
    {
        private static readonly string _corsPolicy = "AllowAny";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(new AuthConfig()
            {
                Secret = this.Configuration.GetSection("AuthConfig").GetValue<string>("Secret")
            });

            //            var mongodbClient = new MongoClient(this.Configuration.GetConnectionString("MongoDb"));

            services.AddDbContext<BLBDatabaseContext>(options => 
                options.UseMySql(this.Configuration.GetConnectionString("MySql")));
            // add connection stuff because life
//            services.AddSingleton<IMongoClient>(mongodbClient);
            services.AddScoped<ICommandService, MySqlCommandService>();
            services.AddScoped<IUserService, MySqlUserService>();
            services.AddScoped<ITimedMessageService, MySqlTimedMessageService>();
            services.AddScoped<IWebhookSubscriberService, MySqlWebhookSubscriberService>();
            services.AddScoped<CommandValidator>();
            services.AddScoped<UserValidator>();
            services.AddScoped<TimedMessageValidator>();
            services.AddScoped<WebhookManager>();

            services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", builder =>
                {
                    builder.AllowAnyOrigin()
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
            });

            services.AddControllers();
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
            
            app.UseCors(_corsPolicy);
//            app.UseAuthorization();

            app.UseMiddleware<AuthenticationMiddleware>();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
