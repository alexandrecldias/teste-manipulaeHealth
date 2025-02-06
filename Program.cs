using DesafioBackEndRDManipulacao.Data;
using DesafioBackEndRDManipulacao.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Configurar serviços
builder.Services.AddDbContext<YouTubeDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("YouTubeDbConnection")));
builder.Services.AddScoped<YouTubeService>();
builder.Services.AddScoped<VideoService>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
