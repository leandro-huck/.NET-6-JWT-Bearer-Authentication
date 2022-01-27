using System.Globalization;
using Microsoft.Extensions.Configuration;
namespace SecureClient
{
    public class AuthConfig
    {
        public AuthConfig()
        {
            Instance = "";
            ClientId = "";
            TenantId = "";
            ClientSecret = "";
            BaseAddress = "";
            ResourceId = "";
        }
        public string Instance { get; set; }
        public string ClientId { get; set; }
        public string TenantId { get; set; }
        public string Authority
        {
            get
            {
                return String.Format(CultureInfo.InvariantCulture, Instance, TenantId);
            }
        }
        public string ClientSecret { get; set; }
        public string BaseAddress { get; set; }
        public string ResourceId { get; set; }

        public static AuthConfig ReadJsonFromFile(string path)
        {
            IConfiguration Configuration;
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(path);
            Configuration = builder.Build();

            return Configuration.Get<AuthConfig>();
        }

    }
}