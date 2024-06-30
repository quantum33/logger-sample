using LogTest.Entities;
using LogTest.Loggers;
using LogTest.Temp;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddLogging(b => b.AddCustomFormatter(_ => { }));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IMyScopedService, MyScopedService>();
builder.Services.AddSingleton<IMessageHandler, MessageHandler>();

var app = builder.Build();


// ILogger<Program> logger = app.Services.GetRequiredService<ILogger<Program>>();
using ILoggerFactory loggerFactory = LoggerFactory.Create(build => 
    build.AddCustomFormatter(options => options.CustomPrefix = " ~~~~~ "));
var logger = loggerFactory.CreateLogger<Program>();

Supa supa = new("myValue_1", "myValue_2");
Foo foo = new("bar1", "baz1");

logger.LogInformation("This is my SUPA: {supa} and FOO: {foo}", supa, foo);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var summaries = new[] { "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching" };

app.MapGet("/weatherforecast", () =>
    {
        var forecast = Enumerable.Range(1, 5).Select(index =>
                new WeatherForecast
                (
                    DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                    Random.Shared.Next(-20, 55),
                    summaries[Random.Shared.Next(summaries.Length)]
                ))
            .ToArray();
        return forecast;
    })
    .WithName("GetWeatherForecast")
    .WithOpenApi();

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}