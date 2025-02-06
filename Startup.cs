using DesafioBackEndRDManipulacao.Data;
// Startup.cs
using Microsoft.EntityFrameworkCore;
using DesafioBackEndRDManipulacao.Services; // Adicione o namespace dos seus serviços


namespace DesafioBackEndRDManipulacao
{

    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<YouTubeDbContext>(options =>
                options.UseSqlite(Configuration.GetConnectionString("YouTubeDbConnection")));

            services.AddControllers();

            //Injeção de dependência dos seus serviços
            services.AddScoped<YouTubeService>();
            services.AddScoped<VideoService>();

        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
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
