using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaDatos.SQL
{
    public static partial class Query
    {
        public static partial class Asiento
        {
            //Test
            public const string Update = @"
            UPDATE accounting_entries SET active = FALSE WHERE accounting_entry_id = @accounting_entry_id LIMIT 1;
        ";
        }
    }
}
