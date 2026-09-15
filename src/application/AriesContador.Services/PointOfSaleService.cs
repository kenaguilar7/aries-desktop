using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AriesContador.Core;
using AriesContador.Core.Models.PointOfSale;
using AriesContador.Core.Services;

namespace AriesContador.Services
{
    public class PointOfSaleService : IPointOfSaleService
    {
        public const decimal DefaultLowStockMinimum = 5m;

        private readonly IUnitOfWork _unitOfWork;

        public PointOfSaleService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public Task<IEnumerable<Product>> GetProductsAsync(string companyId, CancellationToken cancellationToken = default)
        {
            RequireCompany(companyId);
            return _unitOfWork.ProductRepository.FindByCompanyIdAsync(companyId, cancellationToken);
        }

        public Task<Product> FindProductAsync(int id, CancellationToken cancellationToken = default)
        {
            return _unitOfWork.ProductRepository.GetByIdAsync(id, cancellationToken);
        }

        public Task<Product> FindProductByBarcodeAsync(string companyId, string barcode, CancellationToken cancellationToken = default)
        {
            RequireCompany(companyId);
            if (string.IsNullOrWhiteSpace(barcode))
                throw new InvalidOperationException("El código de barras es requerido");
            return _unitOfWork.ProductRepository.FindByBarcodeAsync(companyId, NormalizeBarcode(barcode), cancellationToken);
        }

        public async Task CreateProductAsync(Product product, CancellationToken cancellationToken = default)
        {
            ValidateProduct(product);
            product.Barcode = NormalizeBarcode(product.Barcode);
            product.Active = true;
            var existing = await _unitOfWork.ProductRepository.FindByBarcodeAsync(
                product.CompanyId, product.Barcode, cancellationToken).ConfigureAwait(false);
            if (existing != null)
                throw new InvalidOperationException("Ya existe un producto con ese código de barras");
            await _unitOfWork.ProductRepository.AddAsync(product, cancellationToken).ConfigureAwait(false);
        }

        public async Task UpdateProductAsync(Product product, CancellationToken cancellationToken = default)
        {
            ValidateProduct(product);
            if (product.Id <= 0)
                throw new InvalidOperationException("Producto inválido");
            var current = await RequireProductAsync(product.Id, product.CompanyId, cancellationToken).ConfigureAwait(false);
            product.Barcode = NormalizeBarcode(product.Barcode);
            var existing = await _unitOfWork.ProductRepository.FindByBarcodeAsync(
                product.CompanyId, product.Barcode, cancellationToken).ConfigureAwait(false);
            if (existing != null && existing.Id != product.Id)
                throw new InvalidOperationException("Ya existe un producto con ese código de barras");
            product.Active = current.Active;
            await _unitOfWork.ProductRepository.UpdateAsync(product, cancellationToken).ConfigureAwait(false);
        }

        public async Task DeleteProductAsync(int id, CancellationToken cancellationToken = default)
        {
            var product = await _unitOfWork.ProductRepository.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);
            if (product == null)
                throw new InvalidOperationException("Producto no encontrado");
            await _unitOfWork.ProductRepository.RemoveAsync(product, cancellationToken).ConfigureAwait(false);
        }

        public Task<IEnumerable<SalesRegister>> GetRegistersAsync(string companyId, CancellationToken cancellationToken = default)
        {
            RequireCompany(companyId);
            return _unitOfWork.SalesRegisterRepository.FindByCompanyIdAsync(companyId, cancellationToken);
        }

        public Task<SalesRegister> FindRegisterAsync(int id, CancellationToken cancellationToken = default)
        {
            return _unitOfWork.SalesRegisterRepository.GetByIdAsync(id, cancellationToken);
        }

        public async Task CreateRegisterAsync(SalesRegister register, CancellationToken cancellationToken = default)
        {
            ValidateRegister(register);
            register.Code = register.Code.Trim();
            register.Name = register.Name.Trim();
            register.Active = true;
            var existing = await _unitOfWork.SalesRegisterRepository.FindByCompanyIdAsync(register.CompanyId, cancellationToken)
                .ConfigureAwait(false);
            if (existing.Any(r => string.Equals(r.Code, register.Code, StringComparison.OrdinalIgnoreCase)))
                throw new InvalidOperationException("Ya existe una caja con ese código");
            await _unitOfWork.SalesRegisterRepository.AddAsync(register, cancellationToken).ConfigureAwait(false);
        }

        public async Task UpdateRegisterAsync(SalesRegister register, CancellationToken cancellationToken = default)
        {
            ValidateRegister(register);
            if (register.Id <= 0)
                throw new InvalidOperationException("Caja inválida");
            await RequireRegisterAsync(register.Id, register.CompanyId, cancellationToken).ConfigureAwait(false);
            register.Code = register.Code.Trim();
            register.Name = register.Name.Trim();
            var existing = await _unitOfWork.SalesRegisterRepository.FindByCompanyIdAsync(register.CompanyId, cancellationToken)
                .ConfigureAwait(false);
            if (existing.Any(r => r.Id != register.Id && string.Equals(r.Code, register.Code, StringComparison.OrdinalIgnoreCase)))
                throw new InvalidOperationException("Ya existe una caja con ese código");
            await _unitOfWork.SalesRegisterRepository.UpdateAsync(register, cancellationToken).ConfigureAwait(false);
        }

        public async Task DeleteRegisterAsync(int id, CancellationToken cancellationToken = default)
        {
            var register = await _unitOfWork.SalesRegisterRepository.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);
            if (register == null)
                throw new InvalidOperationException("Caja no encontrada");
            var open = await _unitOfWork.SalesRegisterRepository.GetOpenSessionAsync(id, cancellationToken).ConfigureAwait(false);
            if (open != null)
                throw new InvalidOperationException("No se puede eliminar una caja con sesión abierta");
            await _unitOfWork.SalesRegisterRepository.RemoveAsync(register, cancellationToken).ConfigureAwait(false);
        }

        public Task<SalesRegisterSession> GetOpenSessionAsync(int registerId, CancellationToken cancellationToken = default)
        {
            return _unitOfWork.SalesRegisterRepository.GetOpenSessionAsync(registerId, cancellationToken);
        }

        public async Task<SalesRegisterSession> OpenSessionAsync(
            int registerId,
            decimal openingAmount,
            string notes,
            int userId,
            CancellationToken cancellationToken = default)
        {
            if (openingAmount < 0)
                throw new InvalidOperationException("El monto inicial no puede ser negativo");
            var register = await _unitOfWork.SalesRegisterRepository.GetByIdAsync(registerId, cancellationToken)
                .ConfigureAwait(false);
            if (register == null || !register.Active)
                throw new InvalidOperationException("Caja no encontrada");
            var open = await _unitOfWork.SalesRegisterRepository.GetOpenSessionAsync(registerId, cancellationToken)
                .ConfigureAwait(false);
            if (open != null)
                throw new InvalidOperationException("Ya hay una caja abierta");

            var session = new SalesRegisterSession
            {
                SalesRegisterId = registerId,
                CompanyId = register.CompanyId,
                OpenedAt = DateTime.Now,
                OpeningAmount = openingAmount,
                OpeningNotes = notes ?? string.Empty,
                ExpectedClosingAmount = openingAmount,
                CreatedBy = userId,
                UpdatedBy = userId,
                Active = true,
                RegisterCode = register.Code,
                RegisterName = register.Name
            };
            await _unitOfWork.SalesRegisterRepository.AddSessionAsync(session, cancellationToken).ConfigureAwait(false);
            return session;
        }

        public async Task<SalesRegisterSession> CloseSessionAsync(
            int registerId,
            decimal declaredAmount,
            string notes,
            int userId,
            CancellationToken cancellationToken = default)
        {
            if (declaredAmount < 0)
                throw new InvalidOperationException("El monto declarado no puede ser negativo");
            var session = await _unitOfWork.SalesRegisterRepository.GetOpenSessionAsync(registerId, cancellationToken)
                .ConfigureAwait(false);
            if (session == null)
                throw new InvalidOperationException("No hay una caja abierta");

            session.ClosedAt = DateTime.Now;
            session.ExpectedClosingAmount = session.OpeningAmount + session.CashSales;
            session.DeclaredClosingAmount = declaredAmount;
            session.Difference = declaredAmount - session.ExpectedClosingAmount;
            session.ClosingNotes = notes ?? string.Empty;
            session.UpdatedBy = userId;
            await _unitOfWork.SalesRegisterRepository.CloseSessionAsync(session, cancellationToken).ConfigureAwait(false);
            return session;
        }

        public Task<IEnumerable<SalesRegisterSession>> GetSessionHistoryAsync(string companyId, CancellationToken cancellationToken = default)
        {
            RequireCompany(companyId);
            return _unitOfWork.SalesRegisterRepository.GetSessionHistoryAsync(companyId, cancellationToken);
        }

        public Task<IEnumerable<Sale>> GetSalesByCompanyAsync(string companyId, CancellationToken cancellationToken = default)
        {
            RequireCompany(companyId);
            return _unitOfWork.SaleRepository.FindByCompanyIdAsync(companyId, cancellationToken);
        }

        public Task<IEnumerable<Sale>> GetSalesBySessionAsync(int sessionId, CancellationToken cancellationToken = default)
        {
            return _unitOfWork.SaleRepository.FindBySessionIdAsync(sessionId, cancellationToken);
        }

        public async Task<Sale> CreateSaleAsync(Sale sale, CancellationToken cancellationToken = default)
        {
            if (sale == null)
                throw new InvalidOperationException("La venta es requerida");
            RequireCompany(sale.CompanyId);
            if (sale.SalesRegisterId <= 0)
                throw new InvalidOperationException("Debe indicar la caja");
            if (sale.Lines == null || sale.Lines.Count == 0)
                throw new InvalidOperationException("La venta debe tener al menos un producto");

            if (sale.PaymentMethod != PaymentMethod.Efectivo && string.IsNullOrWhiteSpace(sale.PaymentReference))
                throw new InvalidOperationException("La referencia de pago es requerida");

            foreach (var raw in sale.Lines)
            {
                if (raw.ProductId <= 0)
                    throw new InvalidOperationException("Línea de venta sin producto");
            }

            var productIds = sale.Lines.Select(l => l.ProductId).Distinct().ToList();
            var registerTask = RequireRegisterAsync(sale.SalesRegisterId, sale.CompanyId, cancellationToken);
            var sessionTask = _unitOfWork.SalesRegisterRepository.GetOpenSessionAsync(sale.SalesRegisterId, cancellationToken);
            var mapTask = _unitOfWork.PosAccountMapRepository.GetByCompanyIdAsync(sale.CompanyId, cancellationToken);
            var productsTask = _unitOfWork.ProductRepository.FindByIdsAsync(sale.CompanyId, productIds, cancellationToken);

            var register = await registerTask.ConfigureAwait(false);
            var session = await sessionTask.ConfigureAwait(false);
            if (session == null)
                throw new InvalidOperationException("Debe abrir la caja antes de vender");

            var map = await mapTask.ConfigureAwait(false);
            var productsById = (await productsTask.ConfigureAwait(false)).ToDictionary(p => p.Id);
            var remainingStock = productsById.ToDictionary(pair => pair.Key, pair => pair.Value.Stock);
            var taxRate = map?.TaxRate ?? PosTax.DefaultRate;
            var pricesIncludeTax = map == null || map.PricesIncludeTax;
            var prepared = new List<SaleLine>();
            foreach (var raw in sale.Lines)
            {
                Product product;
                if (!productsById.TryGetValue(raw.ProductId, out product) || product == null || !product.Active)
                    throw new InvalidOperationException("Producto no encontrado");
                var line = PrepareLine(product, raw, taxRate, pricesIncludeTax);
                decimal remaining;
                remainingStock.TryGetValue(product.Id, out remaining);
                if (remaining < line.StockToDecrement)
                    throw new InvalidOperationException("Stock insuficiente para " + product.Name);
                remainingStock[product.Id] = remaining - line.StockToDecrement;
                prepared.Add(line);
            }

            sale.SessionId = session.Id;
            sale.Lines = prepared;
            sale.Total = prepared.Sum(l => l.LineTotal);
            sale.NetAmount = prepared.Sum(l => l.NetAmount);
            sale.TaxAmount = prepared.Sum(l => l.TaxAmount);
            sale.CostAmount = prepared.Sum(l => l.CostAmount);
            sale.SoldAt = sale.SoldAt == default ? DateTime.Now : sale.SoldAt;
            sale.UpdatedBy = sale.CreatedBy;
            sale.Active = true;
            sale.RegisterCode = register.Code;
            sale.RegisterName = register.Name;

            await _unitOfWork.SaleRepository.CreateWithEffectsAsync(sale, cancellationToken).ConfigureAwait(false);
            return sale;
        }

        public async Task<PosTodaySalesReport> GetTodaySalesAsync(
            string companyId,
            DateTime? day,
            CancellationToken cancellationToken = default)
        {
            RequireCompany(companyId);
            var start = (day ?? DateTime.Today).Date;
            var salesTask = _unitOfWork.SaleRepository.FindByCompanyAndDateRangeAsync(
                companyId, start, start.AddDays(1), cancellationToken);
            var productsTask = _unitOfWork.ProductRepository.CountActiveByCompanyAsync(companyId, cancellationToken);
            var lowTask = _unitOfWork.ProductRepository.FindLowStockAsync(
                companyId, DefaultLowStockMinimum, cancellationToken);
            var sales = (await salesTask.ConfigureAwait(false)).ToList();
            var products = await productsTask.ConfigureAwait(false);
            var low = await lowTask.ConfigureAwait(false);
            return new PosTodaySalesReport
            {
                SaleCount = sales.Count,
                Total = sales.Sum(s => s.Total),
                CashTotal = sales.Where(s => s.PaymentMethod == PaymentMethod.Efectivo).Sum(s => s.Total),
                CardTotal = sales.Where(s => s.PaymentMethod == PaymentMethod.Tarjeta).Sum(s => s.Total),
                TransferTotal = sales.Where(s => s.PaymentMethod == PaymentMethod.Transferencia).Sum(s => s.Total),
                ProductCount = products,
                LowStockCount = low.Count(),
                RecentSales = sales.Take(10).ToList()
            };
        }

        public Task<IEnumerable<Product>> GetLowStockAsync(string companyId, decimal minimum, CancellationToken cancellationToken = default)
        {
            RequireCompany(companyId);
            if (minimum < 0)
                minimum = DefaultLowStockMinimum;
            return _unitOfWork.ProductRepository.FindLowStockAsync(companyId, minimum, cancellationToken);
        }

        public async Task<IEnumerable<SalesRegisterSession>> GetClosingsAsync(string companyId, CancellationToken cancellationToken = default)
        {
            var history = await GetSessionHistoryAsync(companyId, cancellationToken).ConfigureAwait(false);
            return history.Where(s => s.ClosedAt.HasValue).ToList();
        }

        private static SaleLine PrepareLine(Product product, SaleLine raw, decimal taxRate, bool pricesIncludeTax)
        {
            var line = new SaleLine
            {
                ProductId = product.Id,
                ProductName = product.Name,
                SoldByWeight = product.SoldByWeight,
                UnitPrice = product.Price,
                PricePerKilo = product.PricePerKilo,
                TaxExempt = product.TaxExempt
            };

            if (product.SoldByWeight)
            {
                if (raw.WeightGrams <= 0)
                    throw new InvalidOperationException("Indique el peso en gramos para " + product.Name);
                if (!product.PricePerKilo.HasValue || product.PricePerKilo.Value <= 0)
                    throw new InvalidOperationException("El producto " + product.Name + " no tiene precio por kilo");
                line.WeightGrams = raw.WeightGrams;
                line.Quantity = 0;
                line.LineTotal = Math.Round(product.PricePerKilo.Value * raw.WeightGrams / 1000m, 2);
            }
            else
            {
                var qty = raw.Quantity <= 0 ? 1 : raw.Quantity;
                line.Quantity = qty;
                line.WeightGrams = 0;
                line.LineTotal = Math.Round(product.Price * qty, 2);
            }

            if (line.LineTotal <= 0)
                throw new InvalidOperationException("El importe de la línea debe ser mayor a cero");

            ApplyTaxAndCost(product, line, taxRate, pricesIncludeTax);
            return line;
        }

        private static void ApplyTaxAndCost(Product product, SaleLine line, decimal taxRate, bool pricesIncludeTax)
        {
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

            line.CostAmount = PosTax.LineCost(product, line);
        }

        private async Task<Product> RequireProductAsync(int id, string companyId, CancellationToken cancellationToken)
        {
            var product = await _unitOfWork.ProductRepository.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);
            if (product == null || !product.Active || !string.Equals(product.CompanyId, companyId, StringComparison.Ordinal))
                throw new InvalidOperationException("Producto no encontrado");
            return product;
        }

        private async Task<SalesRegister> RequireRegisterAsync(int id, string companyId, CancellationToken cancellationToken)
        {
            var register = await _unitOfWork.SalesRegisterRepository.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);
            if (register == null || !register.Active || !string.Equals(register.CompanyId, companyId, StringComparison.Ordinal))
                throw new InvalidOperationException("Caja no encontrada");
            return register;
        }

        private static void ValidateProduct(Product product)
        {
            if (product == null)
                throw new InvalidOperationException("El producto es requerido");
            RequireCompany(product.CompanyId);
            if (string.IsNullOrWhiteSpace(product.Barcode))
                throw new InvalidOperationException("El código de barras es requerido");
            if (string.IsNullOrWhiteSpace(product.Name))
                throw new InvalidOperationException("El nombre es requerido");
            if (string.IsNullOrWhiteSpace(product.Category))
                throw new InvalidOperationException("La categoría es requerida");
            if (product.Price <= 0 && !product.SoldByWeight)
                throw new InvalidOperationException("El precio debe ser mayor a cero");
            if (product.SoldByWeight && (!product.PricePerKilo.HasValue || product.PricePerKilo.Value <= 0))
                throw new InvalidOperationException("El precio por kilo debe ser mayor a cero");
            if (product.Stock < 0)
                throw new InvalidOperationException("El stock no puede ser negativo");
            if (product.Cost < 0)
                throw new InvalidOperationException("El costo no puede ser negativo");
        }

        private static void ValidateRegister(SalesRegister register)
        {
            if (register == null)
                throw new InvalidOperationException("La caja es requerida");
            RequireCompany(register.CompanyId);
            if (string.IsNullOrWhiteSpace(register.Code))
                throw new InvalidOperationException("El código de caja es requerido");
            if (string.IsNullOrWhiteSpace(register.Name))
                throw new InvalidOperationException("El nombre de caja es requerido");
        }

        private static void RequireCompany(string companyId)
        {
            if (string.IsNullOrWhiteSpace(companyId))
                throw new InvalidOperationException("La compañía es requerida");
        }

        private static string NormalizeBarcode(string barcode)
        {
            return (barcode ?? string.Empty).Trim().Replace("/", string.Empty);
        }
    }
}
