using System;
using System.Configuration;
using AriesContador.Data;

namespace CapaPresentacion.Conf
{
    public class ConnectionString : IConnectionString
    {
        public string MySQLDefault
        {
            get
            {
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
