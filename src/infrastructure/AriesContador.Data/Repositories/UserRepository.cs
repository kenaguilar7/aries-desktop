using AriesContador.Core.Models.Users;
using AriesContador.Core.Repositories;
using AriesContador.Data.Internal.DataAccess;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AriesContador.Data.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly IConnectionString _connectionString;
        public UserRepository(IConnectionString connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task AddAsync(User entity, CancellationToken cancellationToken = default)
        {
            var dataAccess = new MySqlDataAccess(_connectionString);
            entity.Id = await dataAccess.SaveDataAsync<object, int>("SP_InsertUser", ToInsertParams(entity), cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task<IEnumerable<User>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var dataAccess = new MySqlDataAccess(_connectionString);
            return await dataAccess.LoadDataAsync<User>("SP_GetAllUsers", cancellationToken).ConfigureAwait(false);
        }

        public async Task<User> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var dataAccess = new MySqlDataAccess(_connectionString);
            var output = await dataAccess.LoadDataAsync<User, dynamic>("SP_FindUserById", new { Id = id }, cancellationToken)
                .ConfigureAwait(false);
            return output.FirstOrDefault();
        }

        public async Task<User> FindByUserNameAsync(string userName, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(userName))
                return null;

            var dataAccess = new MySqlDataAccess(_connectionString);
            var output = await dataAccess.ExecuteQueryAsync<User, object>(
                Query.Query.AdministrationQuery.FindUserByUserName,
                new { UserName = userName },
                cancellationToken).ConfigureAwait(false);
            return output.FirstOrDefault();
        }

        public async Task RemoveAsync(User entity, CancellationToken cancellationToken = default)
        {
            entity.Active = false;
            await UpdateAsync(entity, cancellationToken).ConfigureAwait(false);
        }

        public async Task UpdateAsync(User entity, CancellationToken cancellationToken = default)
        {
            var dataAccess = new MySqlDataAccess(_connectionString);
            await dataAccess.SaveDataAsync("SP_UpdateUser", ToUpdateParams(entity), cancellationToken)
                .ConfigureAwait(false);
        }

        private static object ToInsertParams(User entity)
        {
            return new
            {
                entity.UserName,
                UserType = (int)entity.UserType,
                entity.IdNumber,
                entity.Name,
                entity.LastName,
                entity.MiddleName,
                entity.PhoneNumber,
                entity.Mail,
                entity.Memo,
                entity.Password,
                entity.UpdatedBy,
                Active = entity.Active
            };
        }

        private static object ToUpdateParams(User entity)
        {
            return new
            {
                entity.Id,
                entity.UserName,
                UserType = (int)entity.UserType,
                entity.IdNumber,
                entity.Name,
                entity.LastName,
                entity.MiddleName,
                entity.PhoneNumber,
                entity.Mail,
                entity.Memo,
                entity.Password,
                entity.UpdatedBy,
                Active = entity.Active
            };
        }
    }
}
