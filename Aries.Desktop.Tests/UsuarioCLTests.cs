using CapaEntidad.Entidades.Usuarios;
using CapaLogica;
using Xunit;

namespace Aries.Desktop.Tests
{
    public class UsuarioCLTests
    {
        [Fact]
        public void Insert_rejects_blank_name_or_username()
        {
            var cl = new UsuarioCL();
            var user = new Usuario { MyNombre = "  ", UserName = "admin" };

            var ok = cl.Insert(user, user, out var mensaje);

            Assert.False(ok);
            Assert.Contains("blanco", mensaje);
        }
    }
}
