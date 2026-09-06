using AriesContador.Core.Models.Users;
using System;
using System.Collections.Generic;
using System.Text;

namespace AriesContador.Core.Repositories
{
    public interface IUserRepository : IRepository<User>
    {
        User GetById(int id);
        User FindByUserName(string userName);
        IEnumerable<User> GetAll(); 
    }
}
