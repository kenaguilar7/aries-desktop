using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AriesContador.Core;
using AriesContador.Core.Models.PointOfSale;
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

        public Task<IEnumerable<Purchase>> GetPurchasesAsync(string companyId, CancellationToken cancellationToken = default)
        {
            RequireCompany(companyId);
            return _unitOfWork.PurchaseRepository.FindByCompanyIdAsync(companyId, cancellationToken);
        }

        public Task<Purchase> FindPurchaseAsync(int id, CancellationToken cancellationToken = default)
        {
            return _unitOfWork.PurchaseRepository.GetByIdAsync(id, cancellationToken);
        }

        public async Task<Purchase> ConfirmPurchaseAsync(Purchase purchase, CancellationToken cancellationToken = default)
        {
            if (purchase == null)
                throw new InvalidOperationException("La compra es requerida");
            RequireCompany(purchase.CompanyId);
            if (purchase.SupplierId <= 0)
                throw new InvalidOperationException("Debe indicar el proveedor");
            if (string.IsNullOrWhiteSpace(purchase.DocumentNumber))
                throw new InvalidOperationException("El número de factura es requerido");
            if (purchase.Lines == null || purchase.Lines.Count == 0)
                throw new InvalidOperationException("La compra debe tener al menos un producto");
            if (!Enum.IsDefined(typeof(PurchaseSettlement), purchase.PaymentMethod))
                throw new InvalidOperationException("El medio de pago no es válido");
            if (purchase.PaymentMethod != PurchaseSettlement.Cash
                && purchase.PaymentMethod != PurchaseSettlement.OnAccount
                && string.IsNullOrWhiteSpace(purchase.PaymentReference))
                throw new InvalidOperationException("La referencia de pago es requerida");

            var supplier = await RequireSupplierAsync(purchase.SupplierId, purchase.CompanyId, cancellationToken)
                .ConfigureAwait(false);

            purchase.DocumentNumber = purchase.DocumentNumber.Trim();
            var duplicate = await _unitOfWork.PurchaseRepository.FindByDocumentAsync(
                purchase.CompanyId, purchase.SupplierId, purchase.DocumentNumber, cancellationToken)
                .ConfigureAwait(false);
            if (duplicate != null)
                throw new InvalidOperationException("Ya existe una factura con ese número para este proveedor");

            var taxRate = PosTax.DefaultRate;
            const bool pricesIncludeTax = true;
            var prepared = new List<PurchaseLine>();
            foreach (var raw in purchase.Lines)
            {
                if (raw.ProductId <= 0)
                    throw new InvalidOperationException("Línea de compra sin producto");
                var product = await RequireProductAsync(raw.ProductId, purchase.CompanyId, cancellationToken)
                    .ConfigureAwait(false);
                prepared.Add(PrepareLine(product, raw, taxRate, pricesIncludeTax));
            }

            purchase.SupplierName = supplier.Name;
            purchase.Lines = prepared;
            purchase.Total = prepared.Sum(l => l.LineTotal);
            purchase.NetAmount = prepared.Sum(l => l.NetAmount);
            purchase.TaxAmount = prepared.Sum(l => l.TaxAmount);
            purchase.PurchasedAt = purchase.PurchasedAt == default ? DateTime.Now : purchase.PurchasedAt;
            purchase.Notes = NullIfEmpty(purchase.Notes);
            purchase.PaymentReference = NullIfEmpty(purchase.PaymentReference);
            purchase.Status = PurchaseStatus.Confirmed;
            purchase.UpdatedBy = purchase.CreatedBy;
            purchase.Active = true;

            await _unitOfWork.PurchaseRepository.CreateWithEffectsAsync(purchase, cancellationToken)
                .ConfigureAwait(false);
            return purchase;
        }

        private static PurchaseLine PrepareLine(Product product, PurchaseLine raw, decimal taxRate, bool pricesIncludeTax)
        {
            var qty = raw.Quantity;
            if (qty <= 0)
                throw new InvalidOperationException("La cantidad debe ser mayor a cero para " + product.Name);
            var unitPrice = raw.UnitPrice;
            if (unitPrice <= 0)
                throw new InvalidOperationException("El precio unitario debe ser mayor a cero para " + product.Name);

            var line = new PurchaseLine
            {
                ProductId = product.Id,
                ProductName = product.Name,
                Quantity = qty,
                UnitPrice = unitPrice,
                TaxExempt = product.TaxExempt,
                LineTotal = Math.Round(unitPrice * qty, 2, MidpointRounding.AwayFromZero)
            };

            if (line.LineTotal <= 0)
                throw new InvalidOperationException("El importe de la línea debe ser mayor a cero");

            if (pricesIncludeTax)
            {
                var split = PosTax.SplitGross(line.LineTotal, taxRate, product.TaxExempt);
                line.NetAmount = split.Net;
                line.TaxAmount = split.Tax;
            }
            else if (product.TaxExempt || taxRate <= 0)
            {
                line.NetAmount = line.LineTotal;
                line.TaxAmount = 0m;
            }
            else
            {
                line.NetAmount = line.LineTotal;
                line.TaxAmount = Math.Round(line.LineTotal * taxRate, 2, MidpointRounding.AwayFromZero);
                line.LineTotal = line.NetAmount + line.TaxAmount;
            }

            line.CostAmount = line.NetAmount;
            return line;
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

        private async Task<Product> RequireProductAsync(int id, string companyId, CancellationToken cancellationToken)
        {
            var product = await _unitOfWork.ProductRepository.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);
            if (product == null || !product.Active || !string.Equals(product.CompanyId, companyId, StringComparison.Ordinal))
                throw new InvalidOperationException("Producto no encontrado");
            return product;
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
