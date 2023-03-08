using Amazon.S3;
using Microsoft.Extensions.Configuration;
using S3.BaseClass;
using S3.Contract;
using S3.Repository;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<S3Model>(builder.Configuration.GetSection("S3Model"));
//var sett = new S3Model();
//builder.Configuration.GetSection("S3Model").Bind(sett);

builder.Services.AddSingleton<IS3Storage, S3Storage>();

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
