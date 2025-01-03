using AuthenticationService.Interfaces;
using AuthenticationService.Mappings;
using AuthenticationService.Models;
using AuthenticationService.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using NetCoreIntermediate.DbContextService;
using NetCoreIntermediate.Interfaces;
using NetCoreIntermediate.Repositories;
using NetCoreIntermediate.Services;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddDbContext<AuthenticationDbContext>(
    (opt) =>
    {
        opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
    });


builder.Services.AddSwaggerGen(swagger =>
{
    swagger.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Authentication Service",
        Version = "v1",
        Description = "A simple example ASP.NET Core Web API"
    });
    swagger.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: 'Bearer {token}'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    swagger.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    new List<string>()
                }
            });

});

var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["Secret"];

builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
builder.Services.AddScoped<JwtAuthenticationService>(provider =>
{
    var jwtSettings = provider.GetRequiredService<IOptions<JwtSettings>>().Value;
    return new JwtAuthenticationService(jwtSettings.Secret, jwtSettings.Issuer, jwtSettings.Audience);
});

builder.Services.AddSingleton<IJwtToken>(provider =>
{
    var settings = provider.GetRequiredService<IConfiguration>().GetSection("JwtSettings").Get<JwtSettings>();
    return JwtAuthenticationFactory.GetInstance(settings.Secret, settings.Issuer, settings.Audience);
});

builder.Services.AddScoped(typeof(IBaseCrudService<,>), typeof(BaseCrudServiceRepository<,>));
builder.Services.AddScoped<IVerifyUser, VerifyEmailAddressRepository>();
builder.Services.AddAutoMapper(typeof(UpdateMapper<,>));


var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    // Configure Swagger JSON route
    app.UseSwagger(configureSwaggerRoute =>
    {
        configureSwaggerRoute.RouteTemplate = "api/{documentName}/swagger.json";
    });

    // Configure Swagger UI
    app.UseSwaggerUI(endPoint =>
    {
        endPoint.SwaggerEndpoint("/api/v1/swagger.json", "AuthenticationService");
        endPoint.RoutePrefix = "";
    });
}


// Map Controllers (or endpoints, if you are not using MapControllers)
app.MapControllers();

// Run the application
app.Run();