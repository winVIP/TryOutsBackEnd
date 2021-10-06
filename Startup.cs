
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Back_End.Models;
using System.IO;

namespace Back_End
{
    public class Startup
    {
        private IWebHostEnvironment _hostEnvironment;
        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            Configuration = configuration;
            _hostEnvironment = environment;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors();
            services.AddHttpClient();
            services.AddControllers();
            var smt = $"Data Source={Path.Combine(_hostEnvironment.ContentRootPath, "MovieDB", "MovieDBContext.db")}";
            services.AddDbContext<MovieContext>(opt => opt.UseSqlite($"Data Source={Path.Combine(_hostEnvironment.ContentRootPath, "MovieDB", "MovieDB.db")}"));
            //services.AddSwaggerGen(c =>
            //{
            //    c.SwaggerDoc("v1", new OpenApiInfo { Title = "TodoApi", Version = "v1" });
            //});
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseCors(builder => builder.WithOrigins("http://localhost:8080").AllowAnyHeader().AllowAnyMethod());


            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                //app.UseSwagger();
                //app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Back_End v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
