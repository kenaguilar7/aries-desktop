using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AriesContador.Core;
using AriesContador.Core.Models.Permissions;
using AriesContador.Core.Services;

namespace AriesContador.Services
{
    public class PermissionService : IPermissionService
    {
        private readonly IUnitOfWork _unitOfWork;

        public PermissionService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public Task<IList<ModulePermission>> GetModulesAsync(int userId, CancellationToken cancellationToken = default)
        {
            return _unitOfWork.PermissionRepository.GetModulesAsync(userId, cancellationToken);
        }

        public Task<bool> AssignCompaniesAsync(IEnumerable<string> companyCodes, int targetUserId, int updatedByUserId, CancellationToken cancellationToken = default)
        {
            return _unitOfWork.PermissionRepository.AssignCompaniesAsync(companyCodes, targetUserId, updatedByUserId, cancellationToken);
        }

        public Task<bool> RemoveCompaniesAsync(IEnumerable<string> companyCodes, int targetUserId, int updatedByUserId, CancellationToken cancellationToken = default)
        {
            return _unitOfWork.PermissionRepository.RemoveCompaniesAsync(companyCodes, targetUserId, updatedByUserId, cancellationToken);
        }

        public Task<bool> UpdateWindowPermissionsAsync(IList<ModulePermission> modules, int targetUserId, int updatedByUserId, CancellationToken cancellationToken = default)
        {
            return _unitOfWork.PermissionRepository.UpdateWindowPermissionsAsync(modules, targetUserId, updatedByUserId, cancellationToken);
        }
    }
}
