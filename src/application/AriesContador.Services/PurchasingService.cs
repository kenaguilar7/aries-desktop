using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AriesContador.Core;
using AriesContador.Core.Models.Purchases;
using AriesContador.Core.Models.Utils;
using AriesContador.Core.Services;

namespace AriesContador.Services
{
    public class PurchasingService : IPurchasingService
    {
        private readonly IUnitOfWork _unitOfWork;

        public PurchasingService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public Task<IEnumerable<Supplier>> GetSuppliersAsync(string companyId, CancellationToken cancellationToken = default)
        {
            RequireCompany(companyId);
            return _unitOfWork.SupplierRepository.FindByCompanyIdAsync(companyId, cancellationToken);
        }

        public Task<Supplier> FindSupplierAsync(int id, CancellationToken cancellationToken = default)
        {
            return _unitOfWork.SupplierRepository.GetByIdAsync(id, cancellationToken);
        }

        public async Task CreateSupplierAsync(Supplier supplier, CancellationToken cancellationToken = default)
        {
            ValidateSupplier(supplier);
            Normalize(supplier);
            supplier.Active = true;
            await EnsureUniqueNumberIdAsync(supplier, 0, cancellationToken).ConfigureAwait(false);
            await _unitOfWork.SupplierRepository.AddAsync(supplier, cancellationToken).ConfigureAwait(false);
        }

        public async Task UpdateSupplierAsync(Supplier supplier, CancellationToken cancellationToken = default)
        {
            ValidateSupplier(supplier);
            if (supplier.Id <= 0)
                throw new InvalidOperationException("Proveedor inválido");
            var current = await RequireSupplierAsync(supplier.Id, supplier.CompanyId, cancellationToken)
                .ConfigureAwait(false);
            Normalize(supplier);
            await EnsureUniqueNumberIdAsync(supplier, supplier.Id, cancellationToken).ConfigureAwait(false);
            supplier.Active = current.Active;
            await _unitOfWork.SupplierRepository.UpdateAsync(supplier, cancellationToken).ConfigureAwait(false);
        }

        public async Task DeleteSupplierAsync(int id, CancellationToken cancellationToken = default)
        {
            var supplier = await _unitOfWork.SupplierRepository.GetByIdAsync(id, cancellationToken)
                .ConfigureAwait(false);
            if (supplier == null)
                throw new InvalidOperationException("Proveedor no encontrado");
            await _unitOfWork.SupplierRepository.RemoveAsync(supplier, cancellationToken).ConfigureAwait(false);
        }

        private async Task EnsureUniqueNumberIdAsync(Supplier supplier, int excludeId, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(supplier.NumberId))
                return;

            var existing = await _unitOfWork.SupplierRepository.FindByNumberIdAsync(
                supplier.CompanyId, supplier.NumberId, cancellationToken).ConfigureAwait(false);
            if (existing != null && existing.Id != excludeId)
                throw new InvalidOperationException("Ya existe un proveedor con esa identificación");
        }

        private async Task<Supplier> RequireSupplierAsync(int id, string companyId, CancellationToken cancellationToken)
        {
            var supplier = await _unitOfWork.SupplierRepository.GetByIdAsync(id, cancellationToken)
                .ConfigureAwait(false);
            if (supplier == null || !supplier.Active || !string.Equals(supplier.CompanyId, companyId, StringComparison.Ordinal))
                throw new InvalidOperationException("Proveedor no encontrado");
            return supplier;
        }

        private static void ValidateSupplier(Supplier supplier)
        {
            if (supplier == null)
                throw new InvalidOperationException("El proveedor es requerido");
            RequireCompany(supplier.CompanyId);
            if (string.IsNullOrWhiteSpace(supplier.Name))
                throw new InvalidOperationException("El nombre del proveedor es requerido");
            if (!Enum.IsDefined(typeof(IdType), supplier.IdType))
                throw new InvalidOperationException("El tipo de identificación no es válido");
        }

        private static void Normalize(Supplier supplier)
        {
            supplier.Name = supplier.Name.Trim();
            supplier.NumberId = (supplier.NumberId ?? string.Empty).Trim();
            supplier.Email = NullIfEmpty(supplier.Email);
            supplier.Phone = NullIfEmpty(supplier.Phone);
            supplier.Address = NullIfEmpty(supplier.Address);
            supplier.Notes = NullIfEmpty(supplier.Notes);
        }

        private static string NullIfEmpty(string value)
        {
            var trimmed = (value ?? string.Empty).Trim();
            return trimmed.Length == 0 ? null : trimmed;
        }

        private static void RequireCompany(string companyId)
        {
            if (string.IsNullOrWhiteSpace(companyId))
                throw new InvalidOperationException("La compañía es requerida");
        }
    }
}
