using ErgotronChatbotApi;


var builder = WebApplication.CreateBuilder(args);



// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register services
builder.Services.RegisterServices();
builder.Configuration.ConfigureAppSetting();

// Add CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalhost", builder =>
    {
        builder.WithOrigins("http://localhost:44346") // Allow the origin
               .AllowAnyHeader()                     // Allow any header
               .AllowAnyMethod()                  // Allow any HTTP method
               .AllowCredentials();              // Allow cookies/authentication headers if needed
    });
});


var app = builder.Build();

// Use CORS
app.UseCors("AllowLocalhost");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/", () => "Welcome to the API. Use /api/{controller}/{action} to access endpoints.");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
