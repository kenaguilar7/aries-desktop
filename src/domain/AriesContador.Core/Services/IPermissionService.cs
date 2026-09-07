using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AriesContador.Core.Models.Permissions;

namespace AriesContador.Core.Services
{
    public interface IPermissionService
    {
        Task<IList<ModulePermission>> GetModulesAsync(int userId, CancellationToken cancellationToken = default);
        Task<bool> AssignCompaniesAsync(IEnumerable<string> companyCodes, int targetUserId, int updatedByUserId, CancellationToken cancellationToken = default);
        Task<bool> RemoveCompaniesAsync(IEnumerable<string> companyCodes, int targetUserId, int updatedByUserId, CancellationToken cancellationToken = default);
        Task<bool> UpdateWindowPermissionsAsync(IList<ModulePermission> modules, int targetUserId, int updatedByUserId, CancellationToken cancellationToken = default);
    }
}
