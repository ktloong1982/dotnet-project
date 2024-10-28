using Api.Utils;
using log4net;
using log4net.Config;
using log4net.Repository;
using Microsoft.Extensions.Configuration;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;

//configure log4net 
// Ensure the logs directory exists
if (!Directory.Exists("logs"))
{
    Directory.CreateDirectory("logs");
}

ILoggerRepository ? logRepository = null;
var entryAssembly = Assembly.GetEntryAssembly();
if (entryAssembly != null){
 logRepository = LogManager.GetRepository(entryAssembly);
}
if (logRepository != null){
    XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));
}

builder.Logging.ClearProviders();
builder.Logging.AddLog4Net("log4net.config");

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<PartnerService>();
builder.Services.AddSingleton<CryptographyService>();
builder.Services.AddSingleton<ValidatorService>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => 
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
        c.RoutePrefix = "swagger";
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
