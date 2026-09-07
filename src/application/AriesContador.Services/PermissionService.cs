using System.Collections.Generic;
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

        public IList<ModulePermission> GetModules(int userId)
        {
            return _unitOfWork.PermissionRepository.GetModules(userId);
        }

        public bool AssignCompanies(IEnumerable<string> companyCodes, int targetUserId, int updatedByUserId)
        {
            return _unitOfWork.PermissionRepository.AssignCompanies(companyCodes, targetUserId, updatedByUserId);
        }

        public bool RemoveCompanies(IEnumerable<string> companyCodes, int targetUserId, int updatedByUserId)
        {
            return _unitOfWork.PermissionRepository.RemoveCompanies(companyCodes, targetUserId, updatedByUserId);
        }

        public bool UpdateWindowPermissions(IList<ModulePermission> modules, int targetUserId, int updatedByUserId)
        {
            return _unitOfWork.PermissionRepository.UpdateWindowPermissions(modules, targetUserId, updatedByUserId);
        }
    }
}
