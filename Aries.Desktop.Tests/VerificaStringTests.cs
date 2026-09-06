using AriesContador.Core.Models.Utils;
using CapaEntidad.Verificaciones;
using Xunit;

namespace Aries.Desktop.Tests
{
    public class VerificaStringTests
    {
        [Theory]
        [InlineData(IdType.CEDULA_JURIDICA, "3-101-123456", true)]
        [InlineData(IdType.CEDULA_JURIDICA, "3101123456", false)]
        [InlineData(IdType.CEDULA_NACIONAL, "1-2345-6789", true)]
        [InlineData(IdType.CEDULA_NACIONAL, "123456789", false)]
        [InlineData(IdType.DIMEX, "123456789012", true)]
        [InlineData(IdType.DIMEX, "123", false)]
        [InlineData(IdType.NITE, "1234567890", true)]
        [InlineData(IdType.NITE, "123456789", false)]
        public void VerificarID_uses_string_length(IdType type, string id, bool expected)
        {
            var ok = VerificaString.VerificarID(id, type, out _);
            Assert.Equal(expected, ok);
        }

        [Fact]
        public void IsNullOrWhiteSpace_rejects_blank_name()
        {
            var ok = VerificaString.IsNullOrWhiteSpace("  ", "Nombre", out var mensaje);
            Assert.False(ok);
            Assert.Contains("Nombre", mensaje);
        }

        [Theory]
        [InlineData("a@b.com", true)]
        [InlineData("", false)]
        [InlineData("correo@", false)]
        public void ValidarEmail(string email, bool expected)
        {
            Assert.Equal(expected, VerificaString.ValidarEmail(email));
        }
    }
}
