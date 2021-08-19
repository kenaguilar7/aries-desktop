using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AriesContador.Core.Models.Accounts;

namespace CapaPresentacion.Utils
{
    public interface ICallingForm
    {
        bool TransferirCuenta(Account cuenta);

    }
}
