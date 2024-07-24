using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NIHR.ProfileManagement.Domain.Abstractions;
using NIHR.ProfileManagement.Domain.Configuration;
using NIHR.ProfileManagement.Domain.Services;
using NIHR.ProfileManagement.Infrastructure;
using NIHR.ProfileManagement.Infrastructure.MessageBus;
using NIHR.ProfileManagement.Infrastructure.Repository;
using NIHR.ProfileManagement.OutboxProcessor.Configuration;

namespace NIHR.ProfileManagement.OutboxProcessor
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var environmentName = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT");

            await Host.CreateDefaultBuilder()
                .UseEnvironment(environmentName ?? "development")
                .ConfigureAppConfiguration((_, builder) =>
                {
                    builder.Build();
                })
                .ConfigureServices((_, services) =>
                {
                    services.AddHostedService<OutboxProcessorBackgroundService>();

                    services.AddOptions<MessageBusSettings>().Bind(_.Configuration.GetSection("MessageBus"));
                    services.AddOptions<OutboxProcessorSettings>().Bind(_.Configuration.GetSection("OutboxProcessor"));

                    services.AddScoped<IProfileManagementRepository, ProfileManagementRepository>();
                    services.AddScoped<IProfileOutboxRepository, ProfileOutboxRepository>();
                    services.AddScoped<INsipMessageHelper, ProfileKafkaMessageProducer>();
                    services.AddScoped<IOutboxProcessor, ProfileOutboxProcessor>();
                    services.AddScoped<IProfileEventMessagePublisher, ProfileKafkaMessageProducer>();

                    var outboxProcessorConfigurationSection = _.Configuration.GetSection("Data");
                    var databaseSettings = outboxProcessorConfigurationSection.Get<DatabaseSettings>();

                    services.AddDbContext<ProfileManagementDbContext>(options =>
                    {
                        // For local development, username/password included in connection string.
                        // For deployed lambda in AWS, password is retrieved from secret manager
                        var connectionString = "";

                        if (!string.IsNullOrEmpty(databaseSettings?.PasswordSecretName))
                        {
                            // Retrieve password from AWS Secrets.
                            var password = SharedApplicationStartup.GetAwsSecretPassword(databaseSettings.PasswordSecretName);

                            connectionString = $"{databaseSettings.ConnectionString};password={password}";
                        }
                        else
                        {
                            connectionString = databaseSettings?.ConnectionString;
                        }

                        var serverVersion = ServerVersion.AutoDetect(connectionString);


                        options.UseMySql(connectionString, serverVersion);
                    });
                })
                .RunConsoleAsync();
        }
    }
}
