using AriesContador.Services.Security;
using Xunit;

namespace AriesContador.Tests
{
    public class PasswordHasherTests
    {
        [Fact]
        public void Hash_is_longer_than_legacy_varchar_50()
        {
            var hash = PasswordHasher.Hash("96321");
            Assert.True(hash.Length > 50);
            Assert.StartsWith(PasswordHasher.Prefix, hash);
        }

        [Fact]
        public void Verify_accepts_matching_password()
        {
            var hash = PasswordHasher.Hash("secret");
            Assert.True(PasswordHasher.Verify("secret", hash));
            Assert.False(PasswordHasher.Verify("nope", hash));
        }

        [Fact]
        public void LooksHashed_detects_format()
        {
            Assert.True(PasswordHasher.LooksHashed(PasswordHasher.Hash("x")));
            Assert.False(PasswordHasher.LooksHashed("96321"));
            Assert.False(PasswordHasher.LooksHashed(null));
        }
    }
}
