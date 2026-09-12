using AriesContador.Data;
using Xunit;

namespace Aries.Data.Tests
{
    public class MySqlConnectionInfoTests
    {
        [Theory]
        [InlineData("localhost", true)]
        [InlineData("127.0.0.1", true)]
        [InlineData("::1", true)]
        [InlineData("(local)", true)]
        [InlineData("ariescontrol.example.rds.amazonaws.com", false)]
        [InlineData("", false)]
        [InlineData(null, false)]
        public void IsLoopbackHost_detects_local_aliases(string host, bool expected)
        {
            Assert.Equal(expected, MySqlConnectionInfo.IsLoopbackHost(host));
        }

        [Fact]
        public void ShouldAutoMigrate_release_always_applies()
        {
            Assert.True(MySqlConnectionInfo.ShouldAutoMigrate(
                "Server=remote.example;Database=aries",
                debugBuild: false,
                applyMigrationsEnv: null));
        }

        [Fact]
        public void ShouldAutoMigrate_debug_loopback_applies()
        {
            Assert.True(MySqlConnectionInfo.ShouldAutoMigrate(
                "Server=127.0.0.1;Port=3307;Database=aries",
                debugBuild: true,
                applyMigrationsEnv: null));
        }

        [Fact]
        public void ShouldAutoMigrate_debug_remote_skips_without_flag()
        {
            Assert.False(MySqlConnectionInfo.ShouldAutoMigrate(
                "Server=aries-test.example;Port=3306;Database=aries-test",
                debugBuild: true,
                applyMigrationsEnv: null));
        }

        [Fact]
        public void ShouldAutoMigrate_debug_remote_applies_with_flag()
        {
            Assert.True(MySqlConnectionInfo.ShouldAutoMigrate(
                "Server=aries-test.example;Port=3306;Database=aries-test",
                debugBuild: true,
                applyMigrationsEnv: "1"));
        }

        [Fact]
        public void Server_reads_host_from_connection_string()
        {
            Assert.Equal("127.0.0.1", MySqlConnectionInfo.Server(
                "Server=127.0.0.1;Port=3307;User id=kenneth;Database=aries"));
        }
    }
}
