﻿
namespace NIHR.ProfileManagement.CognitoSignUpTrigger.Configuration
{
    public class DatabaseSettings
    {
        public string ConnectionString { get; set; } = $"";

        public string PasswordSecretName { get; set; } = "";
    }
}
