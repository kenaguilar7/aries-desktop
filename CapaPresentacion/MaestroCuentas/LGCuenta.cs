using CapaEntidad.Entidades.Compañias;
using CapaEntidad.Entidades.Cuentas;
using CapaLogica;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaPresentacion.MaestroCuentas
{
    public class LGCuenta
    {

        private CuentaCL _CuentaCL { get; } = new CuentaCL();
        private IEnumerable<Cuenta> cuentas;
        public IEnumerable<Cuenta> Cuentas
        {
            get { return cuentas; }
            set { cuentas = value; }
        }


        public Boolean CargarCuentas(Compañia compañia)
        {
            Cuentas = _CuentaCL.GetAll(compañia);

            return (Cuentas.Count() > 0) ? true : false;
        }
    }
}
