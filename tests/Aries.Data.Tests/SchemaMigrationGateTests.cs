using AriesContador.Core.Models.Users;
using AriesContador.Data.Migrations;
using Xunit;

namespace Aries.Data.Tests
{
    public class SchemaMigrationGateTests
    {
        [Fact]
        public void Only_administrator_may_apply()
        {
            Assert.True(SchemaMigrationGate.CanApply(UserType.Administrador));
            Assert.False(SchemaMigrationGate.CanApply(UserType.Usuario));
            Assert.False(SchemaMigrationGate.CanApply((UserType?)null));
        }
    }
}
