using FakeNewsBackend.Context;
using FakeNewsBackend.Controller;
using FakeNewsBackend.Service;
using FakeNewsBackend.Service.Interface;
using Microsoft.AspNetCore;

var webApplicationOptions = new WebApplicationOptions
{
    ContentRootPath = AppContext.BaseDirectory,
    Args = args,
};    
var builder = WebApplication.CreateBuilder(webApplicationOptions);
// Add services to the container.
builder.WebHost.ConfigureKestrel(c =>
 {
     c.Limits.KeepAliveTimeout = Timeout.InfiniteTimeSpan;
 });

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<WebsiteContext>();
builder.Services.AddDbContext<SimilarityContext>();
builder.Services.AddScoped<HttpClient>();
builder.Services.AddScoped<ISimilarityService, SimilarityService>();
builder.Services.AddScoped<IWebsiteService, WebsiteService>();
builder.Services.AddScoped<HttpController>();
builder.Services.AddScoped<WebController>();
builder.Services.AddScoped<SimilarityController>();

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