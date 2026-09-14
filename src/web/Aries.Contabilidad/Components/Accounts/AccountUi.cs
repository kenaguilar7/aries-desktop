using AriesContador.Core.Models.Accounts;

namespace Aries.Contabilidad.Components.Accounts
{
    public static class AccountUi
    {
        public static string TypeBadge(AccountType type) => type switch
        {
            AccountType.Cuenta_Titulo => "Título",
            AccountType.Cuenta_De_Mayor => "Mayor",
            AccountType.Cuenta_Auxiliar => "Auxiliar",
            _ => type.ToString().Replace('_', ' ')
        };

        public static string TypeLabel(AccountType type) => type.ToString().Replace('_', ' ');

        public static string TagLabel(AccountTag tag) => tag switch
        {
            AccountTag.CostoVenta => "Costo venta",
            _ => tag.ToString()
        };

        public static string Colones(decimal amount) => $"₡{amount:N2}";
    }
}
