using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public string HttpBaseUrl => ConfigurationManager.ConnectionStrings["HttpBaseUrl"].ConnectionString;
    }
}
