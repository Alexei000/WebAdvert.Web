using System;
using System.Net;
using System.Net.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Polly;
using Polly.Extensions.Http;
using WebAdvert.Web.ServiceClients;
using WebAdvert.Web.Services;

namespace AspNetCore
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        private IAsyncPolicy<HttpResponseMessage> GetRetryPolicy(HttpRequestMessage arg)
        {
            return HttpPolicyExtensions.HandleTransientHttpError()
                .OrResult(msg => msg.StatusCode == HttpStatusCode.NotFound)
                .WaitAndRetryAsync(5, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
        }

        private IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPattern(HttpRequestMessage arg)
        {
            return HttpPolicyExtensions.HandleTransientHttpError()
                .CircuitBreakerAsync(3, TimeSpan.FromSeconds(30));
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // services.AddSingleton<IAmazonCognitoIdentityProvider>(cognitoIdentityProvider);
            // services.AddSingleton<CognitoUserPool>(cognitoUserPool);

            services.AddTransient<IFileUploader, S3FileUploader>();

            services.AddCognitoIdentity(config =>
            {
                config.Password = new PasswordOptions
                {
                    RequireDigit = false,
                    RequiredLength = 6,
                    RequiredUniqueChars = 0,
                    RequireLowercase = false,
                    RequireNonAlphanumeric = false,
                    RequireUppercase = false
                };
            });

            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Account/Login";
            });

            services.AddHttpClient<IAdvertApiClient, AdvertApiClient>()
                .AddPolicyHandler(GetRetryPolicy)
                .AddPolicyHandler(GetCircuitBreakerPattern);
            services.AddHttpClient<ISearchApiClient, SearchApiClient>()
                .AddPolicyHandler(GetRetryPolicy)
                .AddPolicyHandler(GetCircuitBreakerPattern);

            services.AddControllersWithViews();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseAuthentication();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
