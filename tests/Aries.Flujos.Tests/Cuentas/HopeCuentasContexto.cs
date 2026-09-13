using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Aries.Flujos.Tests.Infrastructure;
using AriesContador.Core.Models.Accounts;
using AriesContador.Core.Models.Companies;
using AriesContador.Core.Models.JournalEntries;
using AriesContador.Core.Models.PostingPeriods;
using AriesContador.Core.Models.Utils;
using AriesContador.Core.Services;

namespace Aries.Flujos.Tests.Cuentas
{
    internal sealed class HopeCuentasContexto
    {
        public Company Company { get; private set; }
        public PostingPeriod Periodo { get; private set; }
        public Account Bancos { get; private set; }
        public Account CuentasPorCobrar { get; private set; }
        public Account OtrosActivos { get; private set; }
        public Account PasivoCorto { get; private set; }
        public Account IngresoMayor { get; private set; }
        public Account BacColones { get; private set; }
        public Account CxcEfectivo { get; private set; }
        public Account RetencionTarjetas { get; private set; }
        public Account ComisionesTarjetas { get; private set; }
        public Account UsoDatafono { get; private set; }
        public Account IngresosCirugias { get; private set; }

        public static async Task<HopeCuentasContexto> CrearAsync(DesktopServices services)
        {
            var admin = services.Administration;
            var financial = services.Financial;
            var company = FlujoBuilders.NuevaJuridica(
                HopeMuestra.CompaniaNombre,
                HopeMuestra.CompaniaMail,
                createdBy: FlujosDbFixture.AdminUserId);
            await admin.CreateCompanyAsync(company);

            var ctx = new HopeCuentasContexto { Company = company };
            var chart = (await financial.GetAccountsAsync(company.Code)).ToList();
            ctx.Bancos = Find(chart, HopeMuestra.CuentaBancos);
            ctx.CuentasPorCobrar = Find(chart, HopeMuestra.CuentaCuentasPorCobrar);
            ctx.OtrosActivos = Find(chart, HopeMuestra.CuentaOtrosActivos);
            ctx.PasivoCorto = Find(chart, HopeMuestra.CuentaPasivoCorto);
            ctx.IngresoMayor = FindIngresoMayor(chart);
            ctx.ComisionesTarjetas = Find(chart, HopeMuestra.CuentaComisionesTarjetas);

            var companyCode = company.Code;
            ctx.BacColones = await CrearHijaAsync(financial, ctx.Bancos, HopeMuestra.CuentaBacColones, companyCode);
            ctx.CxcEfectivo = await CrearHijaAsync(financial, ctx.CuentasPorCobrar, HopeMuestra.CuentaCxcEfectivo, companyCode);
            ctx.RetencionTarjetas = await CrearHijaAsync(financial, ctx.PasivoCorto, HopeMuestra.CuentaRetencionTarjetas, companyCode);
            ctx.UsoDatafono = await CrearHijaAsync(financial, ctx.OtrosActivos, HopeMuestra.CuentaUsoDatafono, companyCode);
            ctx.IngresosCirugias = await CrearHijaAsync(financial, ctx.IngresoMayor, HopeMuestra.CuentaIngresosCirugias, companyCode);

            var periodo = new PostingPeriod
            {
                Date = HopeMuestra.MesContable,
                Closed = false,
                CompanyId = company.Code,
                UpdatedBy = FlujosDbFixture.AdminUserId,
                Active = true
            };
            await financial.CreatePostingPeriodAsync(periodo);
            ctx.Periodo = periodo;
            return ctx;
        }

        public async Task<JournalEntry> CrearAsiento3Async(IFinancialService financial)
        {
            var number = await financial.CreateJournalEntryConsecutiveAsync(Periodo.Id);
            var entry = new JournalEntry
            {
                Number = number,
                PostingPeriodId = Periodo.Id,
                JournalEntryStatus = JournalEntryStatus.Progress,
                UpdatedBy = FlujosDbFixture.AdminUserId,
                CreatedBy = FlujosDbFixture.AdminUserId,
                Active = true,
                JournalEntryLines =
                {
                    Linea(
                        UsoDatafono.Id,
                        HopeMuestra.CuentaUsoDatafono,
                        HopeMuestra.RefAsiento3,
                        HopeMuestra.DetalleAsiento3,
                        HopeMuestra.FechaAsiento3,
                        HopeMuestra.Asiento3DatafonoDebito,
                        DebOrCred.Debito),
                    Linea(
                        IngresosCirugias.Id,
                        HopeMuestra.CuentaIngresosCirugias,
                        HopeMuestra.RefAsiento3,
                        HopeMuestra.DetalleAsiento3,
                        HopeMuestra.FechaAsiento3,
                        HopeMuestra.Asiento3CirugiasCredito,
                        DebOrCred.Credito)
                }
            };
            entry.ApplyStatusFromBalance();
            await financial.CreateJournalEntryAsync(entry);
            return entry;
        }

        public async Task<JournalEntry> CrearAsiento2Async(IFinancialService financial)
        {
            var number = await financial.CreateJournalEntryConsecutiveAsync(Periodo.Id);
            var entry = new JournalEntry
            {
                Number = number,
                PostingPeriodId = Periodo.Id,
                JournalEntryStatus = JournalEntryStatus.Progress,
                UpdatedBy = FlujosDbFixture.AdminUserId,
                CreatedBy = FlujosDbFixture.AdminUserId,
                Active = true,
                JournalEntryLines =
                {
                    Linea(BacColones.Id, HopeMuestra.CuentaBacColones, HopeMuestra.RefAsiento2, HopeMuestra.DetalleAsiento2, HopeMuestra.FechaAsiento2, HopeMuestra.Asiento2BacDebito, DebOrCred.Debito),
                    Linea(ComisionesTarjetas.Id, HopeMuestra.CuentaComisionesTarjetas, HopeMuestra.RefAsiento2, HopeMuestra.DetalleAsiento2, HopeMuestra.FechaAsiento2, HopeMuestra.Asiento2ComisionDebito, DebOrCred.Debito),
                    Linea(RetencionTarjetas.Id, HopeMuestra.CuentaRetencionTarjetas, HopeMuestra.RefAsiento2, HopeMuestra.DetalleAsiento2, HopeMuestra.FechaAsiento2, HopeMuestra.Asiento2RetencionDebito, DebOrCred.Debito),
                    Linea(CxcEfectivo.Id, HopeMuestra.CuentaCxcEfectivo, HopeMuestra.RefAsiento2, HopeMuestra.DetalleAsiento2, HopeMuestra.FechaAsiento2, HopeMuestra.Asiento2CxcCredito, DebOrCred.Credito)
                }
            };
            entry.ApplyStatusFromBalance();
            await financial.CreateJournalEntryAsync(entry);
            return entry;
        }

        public static JournalEntryLine Linea(
            int accountId,
            string accountName,
            string reference,
            string memo,
            DateTime date,
            decimal amount,
            DebOrCred lado)
        {
            var line = new JournalEntryLine
            {
                AccountId = accountId,
                AccountName = accountName,
                Reference = reference,
                Memo = memo,
                Date = date,
                DebOrCred = lado,
                CreatedBy = FlujosDbFixture.AdminUserId,
                UpdatedBy = FlujosDbFixture.AdminUserId,
                Active = true
            };
            JournalEntryLineAmount.Apply(line, Currency.colones, amount, 1.00m);
            return line;
        }

        private static async Task<Account> CrearHijaAsync(
            IFinancialService financial,
            Account parent,
            string name,
            string companyCode)
        {
            if (parent == null)
                throw new InvalidOperationException("No está la cuenta padre para crear " + name);
            if (string.IsNullOrWhiteSpace(companyCode))
                throw new InvalidOperationException("La compañía no tiene código al crear " + name);

            await financial.EnsureAccountNameAsync(name);

            var child = new Account
            {
                Name = name,
                CompanyId = companyCode,
                FatherAccount = parent.Id,
                Memo = "HOPE C151",
                Editable = true,
                UpdatedBy = FlujosDbFixture.AdminUserId,
                Active = true
            };
            await financial.CreateAccountAsync(child, parent);

            var loaded = child.Id > 0 ? await financial.FindAccountAsync(child.Id) : null;
            if (loaded != null && NamesEqual(loaded.Name, name))
                return loaded;

            var listed = (await financial.GetAccountsAsync(companyCode)).ToList();
            var found = listed.FirstOrDefault(a => NamesEqual(a.Name, name));
            if (found != null)
                return found;

            throw new InvalidOperationException(
                "No se recargó la cuenta '" + name + "'. child.Id=" + child.Id
                + " parent.Id=" + parent.Id
                + " loaded.Name=" + loaded?.Name);
        }

        private static bool NamesEqual(string left, string right)
        {
            return string.Equals((left ?? string.Empty).Trim(), (right ?? string.Empty).Trim(), StringComparison.Ordinal);
        }

        private static Account Find(IEnumerable<Account> accounts, string name)
        {
            return accounts.First(a => NamesEqual(a.Name, name));
        }

        private static Account FindIngresoMayor(IEnumerable<Account> accounts)
        {
            return accounts.First(a =>
                NamesEqual(a.Name, HopeMuestra.CuentaIngresoMayor) && a.AccountType == AccountType.Cuenta_De_Mayor);
        }
    }
}
