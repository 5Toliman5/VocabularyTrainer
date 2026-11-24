using System.Reflection;
using Microsoft.OpenApi.Models;
using VocabularyTrainer.Api.BusinessLogic;
using VocabularyTrainer.Api.Infrastructure.Auth;
using VocabularyTrainer.Api.Middleware;
using VocabularyTrainer.BusinessLogic.Services.Algorithms;
using VocabularyTrainer.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Secrets are not in appsettings.json: user-secrets in development, environment variables on servers
// (deploy-api.ps1 writes them into the IIS site's web.config).
var connectionString = builder.Configuration.GetConnectionString("Default");

if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException(
        "Connection string 'Default' is not configured. Set it with " +
        "'dotnet user-secrets set ConnectionStrings:Default \"...\" --project VocabularyTrainer.Api' " +
        "or the ConnectionStrings__Default environment variable."
    );
}

builder.Services.AddApiBusinessLogic(connectionString);
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApiAuthentication(builder.Configuration);
builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new() { Title = "VocabularyTrainer API", Version = "v1" });

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFile));

    var bearerScheme = new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Description = "Access token from POST api/auth/login.",
        Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" },
    };

    options.AddSecurityDefinition("Bearer", bearerScheme);
    options.AddSecurityRequirement(new OpenApiSecurityRequirement { [bearerScheme] = [] });
});

var app = builder.Build();

await using (var scope = app.Services.CreateAsyncScope())
{
	await scope.ServiceProvider.GetRequiredService<AlgorithmsSeeder>().SeedAsync();
}

app.UseMiddleware<GlobalExceptionHandlerMiddleware>();

app.UseSwagger();
app.UseSwaggerUI();

app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
