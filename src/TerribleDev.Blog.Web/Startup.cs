using System;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Net.Http.Headers;
using HardHat;
using TerribleDev.Blog.Web.Models;
using TerribleDev.Blog.Web.Factories;
using Microsoft.Extensions.Hosting;

namespace TerribleDev.Blog.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            Env = env;
        }

        public IConfiguration Configuration { get; }
        public IWebHostEnvironment Env { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            Func<BlogConfiguration> getBlog = () => Configuration.GetSection("Blog").Get<BlogConfiguration>();
            if (Env.IsDevelopment())
            {
                services.AddTransient(a => getBlog());
            }
            else
            {
                services.AddSingleton(getBlog());
            }
            services.AddSingleton(i => {
                var posts = new BlogFactory().GetAllPosts(Env.IsDevelopment() ? "https://localhost:5001": "https://blog.terrible.dev");
                return BlogCacheFactory.ProjectPostCache(posts);
            });
            services.AddApplicationInsightsTelemetry();
            services.AddControllersWithViews();
            services.AddResponseCompression(a =>
            {
                a.EnableForHttps = true;

            })
            .AddMemoryCache()
            .AddOutputCaching();
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
                app.UseExceptionHandler("/Error");

            }

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
            app.UseRewriter(new Microsoft.AspNetCore.Rewrite.RewriteOptions().AddRedirect("(.*[^/|.xml|.html])$", "$1/", 301));
            app.UseIENoOpen();
            app.UseNoMimeSniff();
            app.UseCrossSiteScriptingFilters();
            app.UseFrameGuard(new FrameGuardOptions(FrameGuardOptions.FrameGuard.SAMEORIGIN));
            app.UseHsts(TimeSpan.FromDays(365), false, preload: true);
            app.UseContentSecurityPolicy(
                new ContentSecurityPolicy()
                {
                    // DefaultSrc = new HashSet<string>() {
                    //    CSPConstants.Self, "https://www.google-analytics.com", "https://www.googletagmanager.com", "https://stats.g.doubleclick.net"
                    // },
                    // ScriptSrc = new HashSet<string>()
                    // {
                    //    CSPConstants.Self, CSPConstants.UnsafeInline, "https://www.google-analytics.com", "https://www.googletagmanager.com", "https://stats.g.doubleclick.net"
                    // },
                    // StyleSrc = new HashSet<string>()
                    // {
                    //    CSPConstants.Self, CSPConstants.UnsafeInline
                    // },
                    UpgradeInsecureRequests = true
                });
            app.UseOutputCaching();
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
