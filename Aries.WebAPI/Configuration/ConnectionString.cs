using AriesContador.Data;

namespace Aries.WebAPI.Configuration
{
    public class ConnectionString : IConnectionString
    {
        private readonly Microsoft.Extensions.Configuration.IConfiguration _configuration;
        public ConnectionString(Microsoft.Extensions.Configuration.IConfiguration configuration)
        {
            this._configuration = configuration;
        }

        public string MySQLDefault => _configuration.GetConnectionString("MySQL.Aries");
    }
}
