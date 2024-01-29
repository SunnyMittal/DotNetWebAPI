using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;
using System.Diagnostics.Metrics;
using System.Diagnostics;
using Elastic.Apm.SerilogEnricher;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Add support to logging with SERILOG
builder.Host.UseSerilog((context, configuration) =>
{
    configuration
        .ReadFrom.Configuration(context.Configuration)
        .Enrich.WithElasticApmCorrelationInfo();
});

// Configure OpenTelemetry with metrics and auto-start.
var tracingOtlpEndpoint = builder.Configuration["OTLP_ENDPOINT_URL"];
builder.Services.AddOpenTelemetry()
    .ConfigureResource(resource => resource
        .AddService(serviceName: builder.Environment.ApplicationName))
    .WithMetrics(metrics => metrics
        .AddAspNetCoreInstrumentation()
        .AddOtlpExporter(otlpOptions => {
            otlpOptions.Endpoint = new Uri(tracingOtlpEndpoint!);
        }))
    .WithTracing(tracing => tracing
        .AddAspNetCoreInstrumentation()
        .AddOtlpExporter(otlpOptions => {
            otlpOptions.Endpoint = new Uri(tracingOtlpEndpoint!);
        }));

//// configure open telemetry
//var tracingOtlpEndpoint = builder.Configuration["OTLP_ENDPOINT_URL"];
//var oTel = builder.Services.AddOpenTelemetry();

//// Configure OpenTelemetry Resources with the application name
//oTel.ConfigureResource(resource => resource
//    .AddService(serviceName: builder.Environment.ApplicationName));

//// Custom metrics for the application
//var weatherForecastGetMeter = new Meter("DotNetWebAPI.WeatherForecast.Get", "1.0.0");
//var countWeatherForecastGetCalls = weatherForecastGetMeter.CreateCounter<int>("DotNetWebAPI.WeatherForecast.Get.count", description: "Counts the number of get calls on DotNetWebAPI.WeatherForecast endpoint");

//// Custom ActivitySource for the application
//var weatherForecastGetActivitySource = new ActivitySource("DotNetWebAPI.WeatherForecast.Get");

//// Add Metrics for ASP.NET Core and our custom metrics and export to Prometheus
//oTel.WithMetrics(metrics => metrics
//    // Metrics provider from OpenTelemetry
//    .AddAspNetCoreInstrumentation()
//    .AddMeter(weatherForecastGetMeter.Name)
//    // Metrics provides by ASP.NET Core in .NET 8
//    .AddMeter("Microsoft.AspNetCore.Hosting")
//    .AddMeter("Microsoft.AspNetCore.Server.Kestrel")
//    .AddOtlpExporter());

//// Add Tracing for ASP.NET Core and our custom ActivitySource and export to Jaeger
//oTel.WithTracing(tracing =>
//{
//    tracing.AddAspNetCoreInstrumentation();
//    tracing.AddHttpClientInstrumentation();
//    tracing.AddSource(weatherForecastGetActivitySource.Name);
//    if (tracingOtlpEndpoint != null)
//    {
//        tracing.AddOtlpExporter(otlpOptions =>
//        {
//            otlpOptions.Endpoint = new Uri(tracingOtlpEndpoint);
//        });
//    }
//    else
//    {
//        tracing.AddConsoleExporter();
//    }
//});

builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Add support to logging request with SERILOG
app.UseSerilogRequestLogging();

app.MapControllers().WithOpenApi();

app.Run();

internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
