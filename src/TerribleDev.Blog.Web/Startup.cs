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

            }).AddMemoryCache().AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
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
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseResponseCompression();
            var cacheTimeOneYear =  TimeSpan.FromDays(365).TotalSeconds;
            app.UseStaticFiles(new StaticFileOptions
            {
                    OnPrepareResponse = ctx =>
                    {
                        ctx.Context.Response.Headers[HeaderNames.CacheControl] =
                            "public,max-age=" + cacheTimeOneYear;
                    }
            });
            var cacheTimeOneMonth =  TimeSpan.FromDays(30).TotalSeconds;
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "img")),
                    OnPrepareResponse = ctx =>
                    {
                        
                        ctx.Context.Response.Headers[HeaderNames.CacheControl] =
                            "public,max-age=" + cacheTimeOneMonth;
                    }
            });
            app.UseRewriter(new Microsoft.AspNetCore.Rewrite.RewriteOptions().AddRedirect("(.*[^/])$", "$1/", 301));
            app.UseMvc();
        }
    }
}
