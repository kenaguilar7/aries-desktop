using CapaDatos.Daos;
using CapaEntidad.Entidades.Seguridad;
using CapaEntidad.Entidades.Usuarios;
using CapaEntidad.Entidades.Ventanas;
using CapaEntidad.Enumeradores;
using Xunit;

namespace Aries.Desktop.Tests
{
    public class GuachiTests
    {
        [Fact]
        public void Administrador_bypasses_window_permissions()
        {
            var admin = new Usuario { TipoUsuario = TipoUsuario.Administrador };

            Assert.True(Guachi.Consultar(admin, VentanaInfo.FormAsientos, CRUDName.Eliminar));
        }

        [Fact]
        public void Usuario_without_module_is_denied()
        {
            var user = new Usuario { TipoUsuario = TipoUsuario.Usuario };

            Assert.False(Guachi.Consultar(user, VentanaInfo.FormAsientos, CRUDName.Insertar));
        }

        [Fact]
        public void Usuario_with_window_crud_is_allowed()
        {
            var ventana = new Ventana
            {
                VentanaInfo = VentanaInfo.FormAsientos,
                CRUDInsert = new CRUDItem(CRUDName.Insertar, true)
            };
            var user = new Usuario
            {
                TipoUsuario = TipoUsuario.Usuario,
                Modulos =
                {
                    new Modulo
                    {
                        TienePermiso = true,
                        LstVentanas = { ventana }
                    }
                }
            };

            Assert.True(Guachi.Consultar(user, VentanaInfo.FormAsientos, CRUDName.Insertar));
            Assert.False(Guachi.Consultar(user, VentanaInfo.FormMaestroCuenta, CRUDName.Insertar));
        }
    }
}
