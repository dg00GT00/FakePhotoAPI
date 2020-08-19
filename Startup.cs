using System;
using FakePhoto.Services;
using FakePhoto.Services.ETagGeneratorService;
using FakePhoto.Services.ETagGeneratorService.Interfaces;
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
            services.AddResponseCaching();
            services.AddSingleton<IStoreKeyGenerator, DefaultStoreKeyGenerator>();
            services.AddSingleton<IETagGenerator, DefaultETagGenerator>(provider =>
            {
                var storeKeyGenerator = provider.GetRequiredService<IStoreKeyGenerator>();
                return new DefaultETagGenerator(storeKeyGenerator);
            });
            services.AddSingleton<IImageBuilderService, HtmlImageBuilderService>();
            services.AddSingleton<IImageSourceService, ImageSourceService>(provider =>
            {
                var imageBuilder = provider.GetRequiredService<IImageBuilderService>();
                return new ImageSourceService(imageBuilder, "FakeImagesDir", ImageType.Png);
            });
            services.AddHttpClient<IFakePhotoService, FakePhotoService>(client =>
            {
                client.BaseAddress = new Uri("https://fakeimg.pl/");
            });
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
            // app.UseHttpsRedirection();

            app.UseStaticFiles();
            app.UseRouting();
            app.UseResponseCaching();
            // app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}