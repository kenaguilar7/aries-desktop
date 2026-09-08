using System;
using System.Configuration;
using AriesContador.Data;

namespace Aries.Desktop.Conf
{
    public class ConnectionString : IConnectionString
    {
        public string MySQLDefault
        {
            get
            {
#if !DEBUG
                var fromEnv = Environment.GetEnvironmentVariable("ARIES_MYSQL_CONNECTION")
                              ?? Environment.GetEnvironmentVariable("ConnectionStrings__MySQLDefault");
                if (!string.IsNullOrWhiteSpace(fromEnv))
                    return fromEnv;
#endif
                var db = ConfigurationManager.ConnectionStrings["DBconnectionString"]
                    ?? ConfigurationManager.ConnectionStrings["DBconnectionstring"];
                if (db == null || string.IsNullOrWhiteSpace(db.ConnectionString))
                    throw new ConfigurationErrorsException(
                        "Falta connectionString 'DBconnectionString' en CapaPresentacion.exe.config.");
                return db.ConnectionString;
            }
        }
    }
}
