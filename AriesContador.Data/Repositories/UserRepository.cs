using AriesContador.Core.Models.Users;
using AriesContador.Core.Repositories;
using AriesContador.Data.Internal.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AriesContador.Data.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly IConnectionString _connectionString;
        public UserRepository(IConnectionString connectionString)
        {
            this._connectionString = connectionString;
        }

        public void Add(User entity)
        {
            MySqlDataAccess dataAccess = new MySqlDataAccess(_connectionString);
            entity.Id = dataAccess.SaveData<User, int>("SP_InsertUser", entity);
        }

        public Task AddAsync(User entity)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<User> GetAll()
        {
            MySqlDataAccess dataAccess = new MySqlDataAccess(_connectionString);
            var output = dataAccess.LoadData<User>("SP_GetAllUsers");
            return output;
        }

        public User GetById(int id)
        {
            MySqlDataAccess dataAccess = new MySqlDataAccess(_connectionString);
            var output = dataAccess.LoadData<User, dynamic>("SP_FindUserById", new { Id = id });
            return output.FirstOrDefault();
        }

        public async Task Remove(User entity)
        {
            throw new NotImplementedException();
        }

        public void Update(User entity)
        {
            MySqlDataAccess dataAccess = new MySqlDataAccess(_connectionString);
            dataAccess.SaveData<User>("SP_UpdateUser", entity); 
        }
    }
}
