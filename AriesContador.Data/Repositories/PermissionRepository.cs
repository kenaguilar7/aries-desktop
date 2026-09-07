using System;
using System.Collections.Generic;
using AriesContador.Core.Models.Permissions;
using AriesContador.Core.Repositories;
using AriesContador.Data.Internal.DataAccess;

namespace AriesContador.Data.Repositories
{
    public class PermissionRepository : IPermissionRepository
    {
        private const string ModulesSql = @"
SELECT
T0.module_id AS Id,
T0.internal_name AS InternalName,
T0.external_name AS ExternalName,
T0.users + 0 AS UserType,
IF(((SELECT COUNT(*) FROM windows_permission T4 WHERE T4.module_id = T0.module_id AND T4.user_id = @UserId AND (T4.u_insert = 1 OR T4.u_update = 1 OR T4.u_remove = 1 OR T4.u_list = 1)) > 0)
OR((SELECT T3.user_type + 0 FROM users T3 WHERE T3.user_id = @UserId LIMIT 1) = 2) , 1,0) AS HasAccess
FROM modules T0 WHERE T0.deleted = 0 AND T0.active = 1";

        private const string WindowsSql = @"
SELECT
T0.window_id AS Id,
T0.internal_name AS InternalName,
T0.external_name AS ExternalName,
T0.comments AS Comments,
T0.active AS Active,
IFNULL((SELECT u_insert FROM windows_permission T10 WHERE T10.window_id = T0.window_id AND T10.module_id = @ModuleId AND T10.user_id = @UserId LIMIT 1), 0) AS CanInsert,
IFNULL((SELECT u_update FROM windows_permission T10 WHERE T10.window_id = T0.window_id AND T10.module_id = @ModuleId AND T10.user_id = @UserId LIMIT 1), 0) AS CanUpdate,
IFNULL((SELECT u_remove FROM windows_permission T10 WHERE T10.window_id = T0.window_id AND T10.module_id = @ModuleId AND T10.user_id = @UserId LIMIT 1), 0) AS CanRemove,
IFNULL((SELECT u_list FROM windows_permission T10 WHERE T10.window_id = T0.window_id AND T10.module_id = @ModuleId AND T10.user_id = @UserId LIMIT 1), 0) AS CanList,
IF(((SELECT COUNT(*) FROM windows_permission T4 WHERE T4.window_id = T0.window_id AND T4.module_id = T0.module_id AND T4.user_id = @UserId AND (T4.u_insert = 1 OR T4.u_update = 1 OR T4.u_remove = 1 OR T4.u_list = 1)) > 0)
OR((SELECT T3.user_type + 0 FROM users T3 WHERE T3.user_id = @UserId LIMIT 1) = 2) , 1,0) AS HasAccess
FROM windows T0 WHERE T0.deleted = 0 AND T0.module_id = @ModuleId";

        private readonly IConnectionString _connectionString;

        public PermissionRepository(IConnectionString connectionString)
        {
            _connectionString = connectionString;
        }

        public IList<ModulePermission> GetModules(int userId)
        {
            var dataAccess = new MySqlDataAccess(_connectionString);
            var modules = dataAccess.ExecuteQuery<ModulePermission, object>(ModulesSql, new { UserId = userId });
            foreach (var module in modules)
            {
                module.Windows = dataAccess.ExecuteQuery<WindowPermission, object>(
                    WindowsSql, new { UserId = userId, ModuleId = module.Id });
            }
            return modules;
        }

        public bool AssignCompanies(IEnumerable<string> companyCodes, int targetUserId, int updatedByUserId)
        {
            var sql = "INSERT INTO companies_permission (user_id,company_id,updated_by) VALUES (@UserId,@CompanyId,@UpdatedBy) "
                      + "ON DUPLICATE KEY UPDATE updated_by = @UpdatedBy , updated_at = NOW(), active = 1";
            var dataAccess = new MySqlDataAccess(_connectionString);
            var pending = 0;
            var done = 0;
            foreach (var code in companyCodes)
            {
                pending++;
                if (dataAccess.ExecuteText(sql, new { UserId = targetUserId, CompanyId = code, UpdatedBy = updatedByUserId }) >= 1)
                    done++;
            }
            return pending == 0 || done == pending;
        }

        public bool RemoveCompanies(IEnumerable<string> companyCodes, int targetUserId, int updatedByUserId)
        {
            var sql = "UPDATE companies_permission SET active = 0, updated_by = @UpdatedBy WHERE user_id = @UserId AND company_id = @CompanyId";
            var dataAccess = new MySqlDataAccess(_connectionString);
            var pending = 0;
            var done = 0;
            foreach (var code in companyCodes)
            {
                pending++;
                if (dataAccess.ExecuteText(sql, new { UserId = targetUserId, CompanyId = code, UpdatedBy = updatedByUserId }) >= 1)
                    done++;
            }
            return pending == 0 || done == pending;
        }

        public bool UpdateWindowPermissions(IList<ModulePermission> modules, int targetUserId, int updatedByUserId)
        {
            const string sql = @"
INSERT INTO windows_permission(user_id, module_id, window_id, u_insert, u_update, u_remove, u_list, updated_by)
VALUES(@UserId, @ModuleId, @WindowId, @CanInsert, @CanUpdate, @CanRemove, @CanList, @UpdatedBy)
ON DUPLICATE KEY UPDATE
u_insert = @CanInsert,
u_update = @CanUpdate,
u_remove = @CanRemove,
u_list = @CanList,
updated_by = @UpdatedBy,
updated_at = NOW()";

            using (var dataAccess = new MySqlDataAccess(_connectionString))
            {
                try
                {
                    dataAccess.StartTransaction();
                    foreach (var module in modules)
                    {
                        if (module?.Windows == null)
                            continue;
                        foreach (var window in module.Windows)
                        {
                            dataAccess.ExecuteTextInTransaction(sql, new
                            {
                                UserId = targetUserId,
                                ModuleId = module.Id,
                                WindowId = window.Id,
                                window.CanInsert,
                                window.CanUpdate,
                                window.CanRemove,
                                window.CanList,
                                UpdatedBy = updatedByUserId
                            });
                        }
                    }
                    dataAccess.CommitTransaction();
                    return true;
                }
                catch (Exception)
                {
                    dataAccess.RollBackTransaction();
                    throw;
                }
            }
        }
    }
}
