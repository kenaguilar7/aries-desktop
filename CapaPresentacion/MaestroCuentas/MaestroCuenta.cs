using AriesContador.Core.Models.Companies;
using CapaEntidad.Entidades.Cuentas;
using CapaEntidad.Entidades.FechaTransacciones;
using CapaLogica;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaPresentacion.MaestroCuentas
{

    public class MaestroCuenta
    {

        private LGCuenta _lGCuenta { get; } = new LGCuenta();


        private Company compañiaActual;
        public IEnumerable<FechaTransaccion> FechaTransaccions { get; internal set; }
        public Company CompañiaActual
        {
            get { return compañiaActual; }
            set
            {
                var result = _lGCuenta.CargarCuentas(value);

                compañiaActual = (!result) ? value : null;
            }
        }
        public IEnumerable<Cuenta> GetAllAccounts()
        {
            return _lGCuenta.Cuentas;
        }

    }
}

