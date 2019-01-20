using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Net.Http.Headers;
using HardHat.Middlewares;
using HardHat;
namespace TerribleDev.Blog.Web
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
            services.AddResponseCompression(a =>
            {
                a.EnableForHttps = true;

            })
            .AddMemoryCache()
            .AddMvcCore()
            .AddCacheTagHelper()
            .AddRazorViewEngine()
            .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.AddOutputCaching();
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
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts(TimeSpan.FromDays(30), false, preload: true);

            }
            app.UseIENoOpen();
            app.UseNoMimeSniff();
            app.UseCrossSiteScriptingFilters();
            app.UseFrameGuard(new FrameGuardOptions(FrameGuardOptions.FrameGuard.SAMEORIGIN));
            app.UseHttpsRedirection();
            app.UseResponseCompression();
            
            var cacheTime = env.IsDevelopment() ? 0 : 31536000;
            app.UseStaticFiles(new StaticFileOptions
            {
                    OnPrepareResponse = ctx =>
                    {
                        ctx.Context.Response.Headers[HeaderNames.CacheControl] =
                            "public,max-age=" + cacheTime;
                    }
            });
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "img")),
                    OnPrepareResponse = ctx =>
                    {
                        ctx.Context.Response.Headers[HeaderNames.CacheControl] =
                            "public,max-age=" + cacheTime;
                    }
            });
            app.UseRewriter(new Microsoft.AspNetCore.Rewrite.RewriteOptions().AddRedirect("(.*[^/|.xml])$", "$1/", 301));
            app.UseOutputCaching();
            app.UseMvc();
        }
    }
}
