using Amazon;
using NIHR.ProfileManagement.Infrastructure.AWS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NIHR.ProfileManagement.Infrastructure
{
    public class SharedApplicationStartup
    {
        public static string GetAwsSecretPassword(string secretName)
        {
            var secretManager = new AwsSecretsManagerClient(RegionEndpoint.EUWest2, secretName);

            var data = secretManager.Load();

            if (data.ContainsKey("password"))
            {
                return data["password"];
            }

            return string.Empty;
        }
    }
}
