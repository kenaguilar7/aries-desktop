using System;
using AriesContador.Core.Models.Companies;
using AriesContador.Core.Models.Users;
using AriesContador.Core.Models.Utils;

namespace Aries.Flujos.Tests.Infrastructure
{
    internal static class FlujoBuilders
    {
        public static string UniqueToken()
        {
            return Guid.NewGuid().ToString("N").Substring(0, 8);
        }

        public static string UniqueUserName(string prefix = "u")
        {
            var name = prefix + UniqueToken();
            return name.Length <= 20 ? name : name.Substring(0, 20);
        }

        public static string UniqueCedulaJuridica()
        {
            return "3-101-" + UniqueToken().Substring(0, 6);
        }

        public static string UniqueCedulaNacional()
        {
            return "1-" + UniqueToken() + "x";
        }

        public static PersonaJuridica NuevaJuridica(
            string nombre,
            string correo,
            string copyFrom = "POR DEFECTO",
            int createdBy = 0)
        {
            var company = new PersonaJuridica(
                numeroId: UniqueCedulaJuridica(),
                tipoID: IdType.CEDULA_JURIDICA,
                nombre: nombre,
                TipoMoneda: CurrencyTypeCompany.Dolares_y_Colones,
                representanteLegal: "Rep Legal",
                IDRepresentante: "1-1111-1111",
                direccion: "San José",
                web: "https://aries.test",
                correo: correo,
                observaciones: "flujo test",
                telefono: new[] { "2222-2222", "8888-8888" });
            company.CopyFrom = copyFrom;
            company.CreatedBy = createdBy;
            company.Active = true;
            return company;
        }

        public static PersonaFisica NuevaFisica(
            string nombre,
            string correo,
            string copyFrom = "POR DEFECTO",
            int createdBy = 0)
        {
            var company = new PersonaFisica(
                numeroId: UniqueCedulaNacional(),
                tipoID: IdType.CEDULA_NACIONAL,
                nombre: nombre,
                TipoMoneda: CurrencyTypeCompany.Solo_Colones,
                apellidoPaterno: "Perez",
                apellidoMaterno: "Mora",
                direccion: "Heredia",
                web: "https://fisica.test",
                correo: correo,
                observaciones: "flujo fisica",
                telefono: new[] { "2260-0000", "7000-0000" });
            company.CopyFrom = copyFrom;
            company.CreatedBy = createdBy;
            company.Active = true;
            return company;
        }

        public static User NuevoUsuario(
            string userName,
            string password,
            UserType tipo = UserType.Usuario,
            bool activo = true)
        {
            return new User
            {
                UserName = userName,
                Password = password,
                UserType = tipo,
                IdNumber = UniqueCedulaNacional(),
                Name = "Nombre " + userName,
                LastName = "Apellido",
                MiddleName = "Segundo",
                PhoneNumber = "8888-0000",
                Mail = userName + "@aries.test",
                Memo = "flujo usuario",
                Active = activo,
                UpdatedBy = 0
            };
        }
    }
}
