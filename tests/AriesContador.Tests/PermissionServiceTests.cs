using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AriesContador.Core.Models.Permissions;
using AriesContador.Services;
using AriesContador.Tests.Fakes;
using Xunit;

namespace AriesContador.Tests
{
    public class PermissionServiceTests
    {
        [Fact]
        public async Task GetModules_forwards_to_repository()
        {
            var uow = new FakeUnitOfWork();
            var stored = (FakePermissionRepository)uow.PermissionRepository;
            stored.Modules.Add(new ModulePermission { Id = 1, ExternalName = "Cuentas", HasAccess = true });
            var svc = new PermissionService(uow);

            var modules = await svc.GetModulesAsync(7);

            Assert.Single(modules);
            Assert.Equal("Cuentas", modules[0].ExternalName);
        }

        [Fact]
        public async Task AssignCompanies_forwards_codes()
        {
            var uow = new FakeUnitOfWork();
            var stored = (FakePermissionRepository)uow.PermissionRepository;
            var svc = new PermissionService(uow);

            Assert.True(await svc.AssignCompaniesAsync(new[] { "C001", "C002" }, 3, 7));
            Assert.Equal(new[] { "C001", "C002" }, stored.AssignedCodes);
            Assert.Equal(3, stored.LastTargetUserId);
            Assert.Equal(7, stored.LastUpdatedBy);
        }
    }
}
