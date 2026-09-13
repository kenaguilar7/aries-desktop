using System;
using AriesContador.Data;

namespace Aries.Flujos.Tests.Infrastructure
{
    internal sealed class FlujosConnectionString : IConnectionString
    {
        public FlujosConnectionString(string connectionString)
        {
            MySQLDefault = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        }

        public string MySQLDefault { get; }
    }
}
