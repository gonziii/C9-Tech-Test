using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using SpotifyDataRepository;
using SpotifyDataRepository.Repositories;
using SpotifyDataRepository.UnitOfWork;
using TechTestBackend;
using TechTestBackend.Services;
using SpotifyRepository = SpotifyDataRepository.Repositories.SpotifyRepository;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddHttpClient<ISpotifyService, SpotifyService>(c => 
    c.BaseAddress = new Uri("https://accounts.spotify.com/"))
    .ConfigurePrimaryHttpMessageHandler(handler =>
        new HttpClientHandler()
        {
            AutomaticDecompression = System.Net.DecompressionMethods.GZip
        });

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddTransient(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddTransient<ISpotifyRepository, SpotifyRepository>();
builder.Services.AddTransient<IUnitOfWork, UnitOfWork>();


builder.Services.AddDbContextFactory<SongStorageContext>(options => options.UseInMemoryDatabase("SongStorage"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();