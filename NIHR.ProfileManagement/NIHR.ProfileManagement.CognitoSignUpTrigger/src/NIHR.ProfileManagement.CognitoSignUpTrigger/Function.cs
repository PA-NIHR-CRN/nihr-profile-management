using Amazon.Lambda.Annotations;
using Amazon.Lambda.CognitoEvents;
using Amazon.Lambda.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NIHR.ProfileManagement.Domain.Abstractions;
using NIHR.ProfileManagement.Domain.Configuration;
using NIHR.ProfileManagement.Domain.Models;
using NIHR.ProfileManagement.Domain.Services;
using NIHR.ProfileManagement.Infrastructure;
using NIHR.ProfileManagement.Infrastructure.MessageBus;
using NIHR.ProfileManagement.Infrastructure.Repository;
using System.Threading;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace NIHR.ProfileManagement.CognitoSignUpTrigger
{
    public class Function
    {
        private readonly IProfileManagementService _profileManagementService;

        private IConfiguration Configuration { get; set; }

        public Function(IProfileManagementService profileManagementService)
        { 
            _profileManagementService = profileManagementService;
        }

        public Function()
        {
            Configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            var serviceCollection = new ServiceCollection();

            ConfigureServices(serviceCollection);

            var serviceProvider = serviceCollection
                .AddLogging(logging => logging.AddConsole())
                .BuildServiceProvider();

            // Get Configuration Service from DI system
            _profileManagementService = serviceProvider.GetService<IProfileManagementService>();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            // Register services with DI system
            services.AddTransient<IProfileManagementService, ProfileManagementService>();
            services.AddTransient<IProfileManagementRepository, ProfileManagementRepository>();
            services.AddTransient<IProfileOutboxRepository, ProfileOutboxRepository>();
            services.AddTransient<INsipMessageHelper, ProfileKafkaMessageProducer>();
            services.AddTransient<IUnitOfWork, UnitOfWork>();

            var messageBusSessings = new MessageBusSettings();

            var dataSettingSection = Configuration.GetSection("Data");

            var dataSettings = dataSettingSection.Get<DatabaseSettings>();

            services.AddOptions<MessageBusSettings>();

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

        [LambdaFunction]
        public async Task<CognitoPreSignupEvent> FunctionHandler(CognitoPreSignupEvent input,
            ILambdaContext context)
        {
            Console.WriteLine($"FunctionHandler: {System.Text.Json.JsonSerializer.Serialize(input)}");

            var firstname = input.Request.UserAttributes.FirstOrDefault(attr => attr.Key == "given_name").Value;
            var lastname = input.Request.UserAttributes.FirstOrDefault(attr => attr.Key == "family_name").Value;

            var createPersonRequest = new CreateProfileRequest
            {
                 Firstname = firstname,
                 Lastname = lastname,
                 sub = input.UserName
            };

            var ctsource = new CancellationTokenSource();
            ctsource.CancelAfter(10000);

            Console.WriteLine($"FunctionHandler: {System.Text.Json.JsonSerializer.Serialize(createPersonRequest)}");

            var result = await _profileManagementService.CreatePersonAsync(createPersonRequest, ctsource.Token);

            input.Response = new CognitoPreSignupResponse { AutoConfirmUser = true };

            return input;
        }
    }
}
