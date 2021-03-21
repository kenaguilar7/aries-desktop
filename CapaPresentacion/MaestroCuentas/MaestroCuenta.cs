using CapaEntidad.Entidades.Compañias;
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


        private Compañia compañiaActual;
        public IEnumerable<FechaTransaccion> FechaTransaccions { get; internal set; }
        public Compañia CompañiaActual
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

