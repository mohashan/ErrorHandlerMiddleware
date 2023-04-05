using ErrorHanlding;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseMiddleware<ErrorHandlerMiddleware>();

var forecastSummary = new List<KeyValuePair<int, string>>
{
 new KeyValuePair<int, string> ( 0,"Freezing" ), 
    new KeyValuePair<int, string>(1,"Bracing"),
    new KeyValuePair<int, string>(2,"Chilly"),
    new KeyValuePair<int, string>(3,"Cool"), 
    new KeyValuePair<int, string>(4,"Mild"), 
    new KeyValuePair<int, string>(5,"Warm"), 
    new KeyValuePair<int, string>(6,"Balmy"), 
    new KeyValuePair<int, string>(7,"Hot"),
    new KeyValuePair<int, string>(8,"Sweltering"),
    new KeyValuePair<int, string>(9,"Scorching")
};

app.MapGet("/weatherforecast/{day}", (int day) =>
{
    var message = string.Empty;
    message = day == 0 ? "it's" : "it will be";
    return Results.Ok(new StandardResponse<KeyValuePair<int,string>>(true, $@"I guess {message} a {forecastSummary[day].Value} day", forecastSummary[day]));
})
.WithName("GetWeatherForecast")
.WithOpenApi();

app.Run();

internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
