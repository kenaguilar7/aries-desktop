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
        public string MySQLDefault => ConfigurationManager.ConnectionStrings["DBconnectionString"].ConnectionString;
    }
}
