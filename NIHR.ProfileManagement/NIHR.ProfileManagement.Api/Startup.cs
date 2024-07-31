using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using NIHR.ProfileManagement.Api.Configuration;
using NIHR.ProfileManagement.Domain.Abstractions;
using NIHR.ProfileManagement.Domain.Services;
using NIHR.ProfileManagement.Infrastructure;
using NIHR.ProfileManagement.Infrastructure.MessageBus;
using NIHR.ProfileManagement.Infrastructure.Repository;
using Swashbuckle.AspNetCore.Filters;
using System.Reflection;
using System.Security.Claims;

namespace NIHR.ProfileManagement.Api;

public class Startup
{
    public Startup(IConfiguration configuration,
        IWebHostEnvironment webHostEnvironment)
    {
        Configuration = configuration;
        _webHostEnvironment = webHostEnvironment;

        using var loggerFactory = LoggerFactory.Create(builder =>
        {
            builder.SetMinimumLevel(LogLevel.Information);
            builder.AddConsole();
        });

        _logger = loggerFactory.CreateLogger<Startup>();
    }

    public IConfiguration Configuration { get; }

    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly ILogger<Startup> _logger;

    // This method gets called by the runtime. Use this method to add services to the container
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();

        var profileManagementApiConfigurationSection = Configuration.GetSection("ProfileManagementApi");

        var profileManagementApiSettings = profileManagementApiConfigurationSection.Get<ProfileManagementApiSettings>();

        services.AddOptions<ProfileManagementApiSettings>().Bind(profileManagementApiConfigurationSection);

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddCookie()
        .AddJwtBearer(options =>
        {
            options.Authority = profileManagementApiSettings.JwtBearer.Authority;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = profileManagementApiSettings.JwtBearer.ValidateIssuerSigningKey,
                ValidateAudience = profileManagementApiSettings.JwtBearer.ValidateAudience
            };

            // If local settings have a configuration value to override jwt token validation, then add
            // some custom handlers to intercept jwt validation events. Note, this bypasses true authentication
            // and should only be used in a local development environment. Claims can be mocked from the same configuration setting
            if (profileManagementApiSettings.JwtBearer.JwtBearerOverride != null
                && profileManagementApiSettings.JwtBearer.JwtBearerOverride.OverrideEvents)
            {
                var events = ConfigureForLocalDevelopment(profileManagementApiSettings);

                if (events != null)
                {
                    options.Events = events;
                }
            }
        });

        services.AddAuthorization();

        // Swagger configuration
        services.AddSwaggerGen(swagger => {
            swagger.SwaggerDoc("v1", new OpenApiInfo()
            {
                Title = "Profile Management API spec.",
                Version = "1.0"
            });

            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

            swagger.IncludeXmlComments(xmlPath);
            swagger.UseAllOfToExtendReferenceSchemas();

            swagger.ExampleFilters();
        });

        services.AddSwaggerExamplesFromAssemblyOf<Startup>();

        services.AddScoped<IProfileManagementRepository, ProfileManagementRepository>();
        services.AddScoped<IProfileManagementService, ProfileManagementService>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IProfileOutboxRepository, ProfileOutboxRepository>();
        services.AddScoped<INsipMessageHelper, ProfileKafkaMessageProducer>();
        services.AddScoped<IProfileEventMessagePublisher, ProfileKafkaMessageProducer>();

        // DbContext
        services.AddDbContext<ProfileManagementDbContext>(options =>
        {
            // For local development, username/password included in connection string.
            // For deployed lambda in AWS, password is retrieved from secret manager
            var connectionString = "";

            if (!string.IsNullOrEmpty(profileManagementApiSettings.Data.PasswordSecretName))
            {
                // Retrieve password from AWS Secrets.
                var password = SharedApplicationStartup.GetAwsSecretPassword(profileManagementApiSettings.Data.PasswordSecretName);

                connectionString = $"{profileManagementApiSettings.Data.ConnectionString};password={password}";
            }
            else
            {
                connectionString = profileManagementApiSettings.Data.ConnectionString;
            }

            var serverVersion = ServerVersion.AutoDetect(connectionString);

            options.UseMySql(connectionString, serverVersion);
        });
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();

            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
                options.RoutePrefix = string.Empty;
            });
        }

        app.UseHttpsRedirection();

        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }

    /// <summary>
    /// This method is to override jwt token validation so that during local development there are no external dependencies, even
    /// for authentication.
    /// Configuration is controlled via appsettings values.
    /// </summary>
    /// <param name="apiSettings"></param>
    /// <returns></returns>
    private JwtBearerEvents? ConfigureForLocalDevelopment(ProfileManagementApiSettings apiSettings)
    {
        if (apiSettings.JwtBearer.JwtBearerOverride == null
            || !apiSettings.JwtBearer.JwtBearerOverride.OverrideEvents)
        {
            return null;
        }

        // Log an error if Jwt Bearer token validation is set to override and the environment is Production.
        if (_webHostEnvironment.IsProduction())
        {
            _logger.LogError("Error: Jwt Bearer Override should not be used in Production environment. Ignoring Jwt Bearer Override.");

            return null;
        }

        return new JwtBearerEvents()
        {
            OnMessageReceived = context =>
            {
                var claims = new List<Claim>();

                foreach (var claimConfig in apiSettings.JwtBearer.JwtBearerOverride.ClaimsOverride)
                {
                    claims.Add(new Claim(claimConfig.Name, claimConfig.Description));
                }

                context.Principal = new ClaimsPrincipal(
                    new ClaimsIdentity(claims, context.Scheme.Name));

                context.Success();

                return Task.CompletedTask;
            },

            OnChallenge = context =>
            {
                return Task.CompletedTask;
            },

            OnForbidden = context =>
            {
                return Task.CompletedTask;
            },

            OnTokenValidated = context =>
            {
                return Task.CompletedTask;
            },

            OnAuthenticationFailed = context =>
            {
                return Task.CompletedTask;
            }
        };
    }
}