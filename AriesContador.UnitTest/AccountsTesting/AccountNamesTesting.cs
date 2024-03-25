using AriesContador.Core.Models.Accounts;
using AriesContador.Core.Models; 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AriesContador.UnitTest.AccountsTesting
{
    public class AccountNamesTesting
    {

        [Fact]
        public void GetPathName_ReturnsCorrectHierarchy()
        {
            // Arrange
            var account1 = new Account { Id = 1, Name = "Account1", FatherAccount = 0 };
            var account2 = new Account { Id = 2, Name = "Account2", FatherAccount = 1 };
            var account3 = new Account { Id = 3, Name = "Account3", FatherAccount = 2 };

            var accounts = new List<Account> { account1, account2, account3 };

            // Act
            var result = account3.GetPathName(accounts);

            // Assert
            Assert.Equal(3, result.Length);
            Assert.Equal("", result[0]);
            Assert.Equal("", result[1]);
            Assert.Equal("Account3", result[2]);
        }

    }
}
