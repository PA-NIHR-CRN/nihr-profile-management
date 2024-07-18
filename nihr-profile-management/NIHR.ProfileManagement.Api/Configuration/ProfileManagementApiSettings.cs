namespace NIHR.ProfileManagement.Api.Configuration
{
    public class ProfileManagementApiSettings
    {
        public JwtBearerSettings JwtBearer { get; set; }

        public DatabaseSettings Data { get; set; }

        public ProfileManagementApiSettings()
        {
            JwtBearer = new JwtBearerSettings();
            Data = new DatabaseSettings();
        }
    }
}
