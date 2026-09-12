using AriesContador.Core.Models.Users;

namespace AriesContador.Data.Migrations
{
    /// <summary>
    /// Las migraciones no se aplican al arrancar el exe ni el API.
    /// Solo un usuario <see cref="UserType.Administrador"/> puede aplicarlas
    /// desde Sistema → Actualizaciones.
    /// </summary>
    public static class SchemaMigrationGate
    {
        public static bool CanApply(UserType userType)
        {
            return userType == UserType.Administrador;
        }

        public static bool CanApply(UserType? userType)
        {
            return userType.HasValue && CanApply(userType.Value);
        }
    }
}
