using System.Text;
using MetroGid.Controllers.Utility.Configuration;
using MetroGid.Controllers.Utility.Interfaces;
using MetroGid.Controllers.Utility.Middleware;
using MetroGid.Core.Exceptions.Handlers;
using MetroGid.Core.Exceptions.Interfaces;
using MetroGid.Core.Exceptions.Loggers;
using MetroGid.Core.Exceptions.Super;
using MetroGid.Core.Interfaces;
using MetroGid.Core.Services;
using MetroGid.Core.Utility.Validators.Handlers;
using MetroGid.DBA.EF.Context;
using MetroGid.DBA.EF.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

class Program
{
    static void Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

        builder.Services.Configure<AppRolesConfig>(
            builder.Configuration.GetSection("AppRoles")
        );

        builder.Services.Configure<JwtConfig>(
            builder.Configuration.GetSection("Jwt")
        );

        builder.Services.AddDbContext<MetroDbContext>(options =>
            options.UseNpgsql(
                builder.Configuration.GetConnectionString("DefaultConnection"),
                o => o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery).SetPostgresVersion(16, 9)
            )
        );

        builder.Services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
            }
        );

        builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }
        ).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = builder.Configuration["Jwt:Issuer"],
                    ValidAudience = builder.Configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)
                    )
                };
            }
        );

        builder.Services.AddSingleton<SuperExceptionHandler, PassThroughHandlerException>();
        builder.Services.AddSingleton<ThrowableDomainAttribsValidator>();
        builder.Services.AddSingleton<ThrowableDomainReferentialityValidator>();
        builder.Services.AddSingleton<IExceptionVisitor, ExceptionMessenger>();

        builder.Services.AddScoped<ITokenService, JwtTokenService>();
        builder.Services.AddScoped<IRoleSwitchingService, RoleSwitchingService>();
        builder.Services.AddScoped<IClientService, ClientService>();
        builder.Services.AddScoped<IChartService, ChartService>();
        builder.Services.AddScoped<IRouteService, RouteService>();

        builder.Services.AddScoped<IRoleSwitchingRepository, EFRoleSwitchingRepository>();
        builder.Services.AddScoped<IClientRepository, EFClientRepository>();
        builder.Services.AddScoped<IChartRepository, EFChartRepository>();
        builder.Services.AddScoped<IRouteRepository, EFRouteRepository>();

        builder.Services.AddSwaggerGen(options =>
            {
                options.AddSecurityDefinition("Bearer",
                    new OpenApiSecurityScheme
                    {
                        In = ParameterLocation.Header,
                        Description = "Введите 'Bearer' + пробел + ваш JWT токен",
                        Name = "Authorization",
                        Type = SecuritySchemeType.ApiKey,
                        Scheme = "Bearer"
                    }
                );

                options.AddSecurityRequirement(
                    new OpenApiSecurityRequirement
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
                            Array.Empty<string>()
                        }
                    }
                );
            }
        );

        WebApplication app = builder.Build();

        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "Metro API v1");
            c.RoutePrefix = "swagger";
        });

        app.UseHttpsRedirection();
        app.UseRouting();
        app.UseMiddleware<GlobalExceptionHandlingMiddleware>();
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseMiddleware<RoleSwitchingMiddleware>();
        app.MapControllers();
        app.Run();
    }
}