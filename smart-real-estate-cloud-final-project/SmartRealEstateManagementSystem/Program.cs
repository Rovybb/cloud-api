using Application;
using Infrastructure;
using Identity;
using Application.AIML;
using Application.Interfaces;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var MyAllowSpecificOrigins = "MyAllowSpecificOrigins";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      policy =>
                      {
                          policy.AllowAnyOrigin();
                          policy.AllowAnyHeader();
                          policy.AllowAnyMethod();
                      });
});

builder.Services.AddApplication();
if (!builder.Environment.IsEnvironment("Test"))
{
    builder.Services.AddDbContext<ApplicationDbContext>(
                options => options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
            );
    builder.Services.AddDbContext<UsersDbContext>(options => options.UseSqlite(builder.Configuration.GetConnectionString("UserConnection")));
}
else
{
    builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseInMemoryDatabase("TestDb"));
    builder.Services.AddDbContext<UsersDbContext>(options => options.UseInMemoryDatabase("TestDb_Identity"));
}
builder.Services.AddIdentity(builder.Configuration);
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "My API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Please insert JWT token into field",
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "bearer"
    });

    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});
// builder.Services.AddSwaggerGen();

if (!builder.Environment.IsEnvironment("Test"))
{
    var predictionModel = new PropertyPricePredictionModel();

    // Use relative paths based on the application's base directory
    var dataPath = Path.Combine(AppContext.BaseDirectory, "Application", "AIML", "Data", "properties_cleaned.csv");
    var modelPath = Path.Combine(AppContext.BaseDirectory, "Application", "AIML", "Data", "model.zip");

    predictionModel.Train(dataPath);
    predictionModel.SaveModel(modelPath);

    builder.Services.AddSingleton(predictionModel);
}

var app = builder.Build();

//Configure middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseSwagger();
//app.UseSwaggerUI();

app.UseStaticFiles();
app.UseRouting();
app.UseCors("MyAllowSpecificOrigins");
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

//var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
//var url = $"http://0.0.0.0:{port}";

await app.RunAsync();

public partial class Program { }
