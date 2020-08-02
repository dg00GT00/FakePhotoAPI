using FakePhoto.Extensions;
using FakePhoto.Services;
using FakePhoto.Services.ImageSourceService;
using FakePhoto.Services.ImageSourceService.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace FakePhoto
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
            services.AddControllers();
            services.AddSingleton<IImageBuilderService, HtmlImageBuilderService>();
            services.AddSingleton<IImageSourceService, ImageSourceService>(provider =>
            {
                var imageBuilder = provider.GetRequiredService<IImageBuilderService>();
                return new ImageSourceService(imageBuilder, "FakeImagesDir", ImageType.Png);
            });
            services.AddHttpClient<IFakePhotoService, FakePhotoService>();
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

            app.UseRouting();

            // app.UseAuthorization();

            app.UseImageCache();
            
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}