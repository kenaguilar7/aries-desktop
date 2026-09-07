using System.Collections.Generic;

namespace AriesContador.Core.Models.Accounts
{
    /// <summary>
    /// Maestro básico (cuentas 1–57) al crear una compañía con "POR DEFECTO".
    /// Misma jerarquía que <c>CompañiaDao.GenerarCuentasDefault</c>.
    /// <see cref="Account.Name"/> es el texto de <c>accounts_names</c>;
    /// <c>CompanyRepository.Add</c> lo resuelve a <c>account_name_id</c> como el API 2.0.
    /// <see cref="Account.Id"/> 1..57 solo sirve para remapear padres al insertar.
    /// </summary>
    public static class DefaultChartOfAccounts
    {
        public const int AccountCount = 57;

        private static readonly string[] CatalogNames =
        {
            "ACTIVO",
            "PASIVO",
            "PATRIMONIO",
            "INGRESO",
            "COSTO VENTA",
            "EGRESO",
            "ACTIVO CORRIENTE",
            "ACTIVO NO CORRIENTE",
            "PASIVO CORTO PLAZO",
            "PASIVO LARGO PLAZO",
            "CAPITAL SOCIAL",
            "RESERVA LEGAL",
            "APORTE SOCIOS",
            "SUPERÁVIT",
            "UTILIDADES ACUMULADAS",
            "CARGOS ADMINISTRATIVOS",
            "GASTOS FINANCIEROS",
            "OTROS GASTOS",
            "CAJA",
            "BANCOS",
            "CUENTAS POR COBRAR",
            "INVENTARIOS",
            "INVERSIONES",
            "OTROS ACTIVOS CORRIENTES",
            "ACTIVOS INTANGIBLES",
            "ACTIVO FIJO",
            "IMPUESTO DIFERIDO",
            "LICENCIAS",
            "OTROS ACTIVOS NO CORRIENTES",
            "SUELDOS Y SALARIOS",
            "AGUINALDO",
            "VACACIONES",
            "PRESTACIONES LEGALES",
            "SEGURO SOCIAL",
            "RIESGOS PROFESIONALES",
            "PAPELERIA Y UTILES DE OFICINA",
            "SERVICIOS PROFESIONALES",
            "SERVICIOS PÚBLICOS",
            "DEPRECIACIONES",
            "MANTENIMIENTO",
            "ALQUILERES",
            "COMBUSTIBLES Y LUBRICANTES",
            "TRANSPORTES",
            "ASEO E HIGIENE",
            "IMPUESTO SOCIEDADES-REGISTRO PÚBLICO",
            "TIMBRE DE EDUCACIÓN",
            "PATENTE MUNICIPAL",
            "ATENCION A CLIENTES",
            "ATENCION A EMPLEADOS",
            "VIÁTICOS Y COMISIONES",
            "PUBLICIDAD",
            "COMISIONES TARJETAS DE CRÉDITO",
            "COMISIÓN DE SERVICIO",
            "DIFERENCIA CAMBIARIA"
        };

        public static IReadOnlyList<Account> Create()
        {
            var accounts = new Account[AccountCount];
            for (var j = 1; j <= AccountCount; j++)
            {
                var spec = Spec(j);
                accounts[j - 1] = new Account
                {
                    Id = j,
                    Name = CatalogNames[spec.NameId - 1],
                    FatherAccount = spec.FatherId == 0 ? (int?)null : spec.FatherId,
                    AccountTag = spec.Tag,
                    AccountType = spec.Type,
                    Editable = false,
                    Active = true
                };
            }

            return accounts;
        }

        private static (int NameId, int FatherId, AccountTag Tag, AccountType Type) Spec(int j)
        {
            if (j <= 6)
                return (j, 0, (AccountTag)j, AccountType.Cuenta_Titulo);
            if (j <= 8)
                return (j, 1, AccountTag.Activo, AccountType.Cuenta_De_Mayor);
            if (j <= 10)
                return (j, 2, AccountTag.Pasivo, AccountType.Cuenta_De_Mayor);
            if (j <= 15)
                return (j, 3, AccountTag.Patrimonio, AccountType.Cuenta_De_Mayor);
            if (j <= 18)
                return (j, 6, AccountTag.Egreso, AccountType.Cuenta_De_Mayor);
            if (j <= 24)
                return (j, 7, AccountTag.Activo, AccountType.Cuenta_Auxiliar);
            if (j <= 29)
                return (j, 8, AccountTag.Activo, AccountType.Cuenta_Auxiliar);
            if (j <= 51)
                return (j, 16, AccountTag.Egreso, AccountType.Cuenta_Auxiliar);
            if (j <= 54)
                return (j, 17, AccountTag.Egreso, AccountType.Cuenta_Auxiliar);
            if (j == 55)
                return (4, 4, AccountTag.Ingreso, AccountType.Cuenta_De_Mayor);
            if (j == 56)
                return (5, 5, AccountTag.CostoVenta, AccountType.Cuenta_De_Mayor);
            return (6, 6, AccountTag.Egreso, AccountType.Cuenta_De_Mayor);
        }
    }
}
