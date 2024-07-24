using Amazon.Lambda.Annotations;
using Amazon.Lambda.CognitoEvents;
using Amazon.Lambda.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NIHR.ProfileManagement.CognitoSignUpTrigger.Configuration;
using NIHR.ProfileManagement.Domain.Abstractions;
using NIHR.ProfileManagement.Domain.Models;
using NIHR.ProfileManagement.Domain.Services;
using NIHR.ProfileManagement.Infrastructure;
using NIHR.ProfileManagement.Infrastructure.Repository;
using System.Threading;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace NIHR.ProfileManagement.CognitoSignUpTrigger
{
    public class Function
    {
        private readonly IProfileManagementService _profileManagementService;

        public Function(IProfileManagementService profileManagementService)
        { 
            _profileManagementService = profileManagementService;
        }

        public async Task<CognitoPreSignupEvent> FunctionHandler(CognitoPreSignupEvent input,
            ILambdaContext context,
            CancellationToken cancellationToken = default)
        {
            var firstname = input.Request.UserAttributes.FirstOrDefault(attr => attr.Key == "given_name").Value;
            var lastname = input.Request.UserAttributes.FirstOrDefault(attr => attr.Key == "family_name").Value;

            var createPersonRequest = new CreateProfileRequest
            {
                 Firstname = firstname,
                 Lastname = lastname,
                 sub = input.UserName
            };

            var result = await _profileManagementService.CreatePersonAsync(createPersonRequest, cancellationToken);

            input.Response = new CognitoPreSignupResponse { AutoConfirmUser = true };

            return input;
        }
    }

    [LambdaStartup]
    public class Startup
    {
        private readonly IConfiguration _configuration;
        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<IProfileManagementRepository, ProfileManagementRepository>();
            services.AddScoped<IProfileManagementService, ProfileManagementService>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IProfileOutboxRepository, ProfileOutboxRepository>();

            var dataSettingSection = _configuration.GetSection("Data");

            var dataSettings = dataSettingSection.Get<DatabaseSettings>;

            // DbContext
            services.AddDbContext<ProfileManagementDbContext>(options =>
            {
                // For local development, username/password included in connection string.
                // For deployed lambda in AWS, password is retrieved from secret manager
                var connectionString = "";

                if (!string.IsNullOrEmpty(dataSettings.PasswordSecretName))
                {
                    // Retrieve password from AWS Secrets.
                    var password = SharedApplicationStartup.GetAwsSecretPassword(dataSettings.PasswordSecretName);

                    connectionString = $"{dataSettings.ConnectionString};password={password}";
                }
                else
                {
                    connectionString = dataSettings.ConnectionString;
                }

                var serverVersion = ServerVersion.AutoDetect(connectionString);

                options.UseMySql(connectionString, serverVersion);
            });
        }
    }
}
