using NETCoreBase.Common.Helpers;
using Xunit;

namespace NETCoreBase.Tests.Helpers
{
    public class CryptHelperTests
    {
        [Fact]
        public void HashPassword_ReturnsNonEmptyString()
        {
            var hash = CryptHelper.HashPassword("mypassword");
            Assert.NotNull(hash);
            Assert.NotEmpty(hash);
        }

        [Fact]
        public void VerifyPassword_CorrectPassword_ReturnsTrue()
        {
            var password = "mypassword123";
            var hash = CryptHelper.HashPassword(password);
            Assert.True(CryptHelper.VerifyPassword(hash, password));
        }

        [Fact]
        public void VerifyPassword_WrongPassword_ReturnsFalse()
        {
            var hash = CryptHelper.HashPassword("correct");
            Assert.False(CryptHelper.VerifyPassword(hash, "wrong"));
        }

        [Fact]
        public void HashPassword_SameInput_ProducesDifferentHashes()
        {
            // PBKDF2 uses a random salt per hash
            var hash1 = CryptHelper.HashPassword("same_password");
            var hash2 = CryptHelper.HashPassword("same_password");
            Assert.NotEqual(hash1, hash2);
        }

        [Fact]
        public void VerifyPassword_BothHashes_VerifyCorrectly()
        {
            var password = "same_password";
            var hash1 = CryptHelper.HashPassword(password);
            var hash2 = CryptHelper.HashPassword(password);
            Assert.True(CryptHelper.VerifyPassword(hash1, password));
            Assert.True(CryptHelper.VerifyPassword(hash2, password));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void HashPassword_NullOrWhitespace_ReturnsInput(string input)
        {
            Assert.Equal(input, CryptHelper.HashPassword(input));
        }

        [Fact]
        public void VerifyPassword_EmptyHash_ReturnsFalse()
        {
            Assert.False(CryptHelper.VerifyPassword(string.Empty, "password"));
        }

        [Fact]
        public void VerifyPassword_EmptyProvided_ReturnsFalse()
        {
            var hash = CryptHelper.HashPassword("real_password");
            Assert.False(CryptHelper.VerifyPassword(hash, string.Empty));
        }
    }
}
