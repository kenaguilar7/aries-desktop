using System.Collections.Generic;
using AriesContador.Core.Models.Permissions;

namespace AriesContador.Core.Services
{
    public interface IPermissionService
    {
        IList<ModulePermission> GetModules(int userId);
        bool AssignCompanies(IEnumerable<string> companyCodes, int targetUserId, int updatedByUserId);
        bool RemoveCompanies(IEnumerable<string> companyCodes, int targetUserId, int updatedByUserId);
        bool UpdateWindowPermissions(IList<ModulePermission> modules, int targetUserId, int updatedByUserId);
    }
}
