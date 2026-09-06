using AriesContador.Core.Models.Companies;
using AriesContador.Core.Models.Utils;
using CapaEntidad.Entidades.Usuarios;
using CapaLogica;
using Xunit;

namespace Aries.Desktop.Tests
{
    public class CompañiaCLTests
    {
        [Fact]
        public void Insert_rejects_invalid_cedula_before_database()
        {
            var cl = new CompañiaCL();
            var company = new Company
            {
                NumberId = "1",
                IdType = IdType.CEDULA_JURIDICA,
                CompanyName = "Demo",
                Mail = "a@b.com"
            };

            var ok = cl.Insert(company, new Usuario(), company, out var mensaje);

            Assert.False(ok);
            Assert.Contains("cédula", mensaje.ToLowerInvariant());
        }

        [Fact]
        public void Insert_rejects_blank_name()
        {
            var cl = new CompañiaCL();
            var company = new Company
            {
                NumberId = "3-101-123456",
                IdType = IdType.CEDULA_JURIDICA,
                CompanyName = "  ",
                Mail = "a@b.com"
            };

            var ok = cl.Insert(company, new Usuario(), company, out var mensaje);

            Assert.False(ok);
            Assert.Contains("Nombre", mensaje);
        }
    }
}
