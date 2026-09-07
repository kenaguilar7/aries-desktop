using AriesContador.Core.Models.Users;
using Aries.Reporting.Entidades.Usuarios;
using Aries.Reporting.Enumeradores;

namespace Aries.Reporting.Mappers
{
    public static class UserMapper
    {
        public static Usuario ToUsuario(User user)
        {
            if (user == null)
                return null;

            return new Usuario
            {
                Id = user.Id,
                UsuarioId = user.Id.ToString(),
                UserName = user.UserName,
                TipoUsuario = (TipoUsuario)user.UserType,
                MyNombre = user.Name,
                MyApellidoPaterno = user.LastName,
                MyApellidoMaterno = user.MiddleName,
                MyCedula = user.IdNumber,
                MyTelefono = user.PhoneNumber,
                MyMail = user.Mail,
                MyNotas = user.Memo,
                MyActivo = user.Active
            };
        }
    }
}
