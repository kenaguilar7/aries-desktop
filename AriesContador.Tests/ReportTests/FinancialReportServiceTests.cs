using System;
using System.Linq;
using AriesContador.Core.Models.Accounts;
using AriesContador.Core.Models.JournalEntries;
using AriesContador.Core.Models.Utils;
using AriesContador.Services;
using AriesContador.Tests.Fakes;
using Xunit;

namespace AriesContador.Tests.ReportTests
{
    public class FinancialReportServiceTests
    {
        [Fact]
        public void BalanceComprobacion_splits_balances_by_DebOCred()
        {
            var uow = new FakeUnitOfWork();
            uow.Accounts.AccountsWithBalance.Add(new Account
            {
                Name = "Caja",
                PathDirection = "Activo¡Caja",
                DebOCred = DebOrCred.Debito,
                PriorBalance = 100,
                DebitBalance = 40,
                CreditBalance = 10
            });
            uow.Accounts.AccountsWithBalance.Add(new Account
            {
                Name = "Ingresos",
                PathDirection = "Ingreso¡Ventas",
                DebOCred = DebOrCred.Credito,
                PriorBalance = 200,
                DebitBalance = 5,
                CreditBalance = 50
            });

            var report = new FinancialReportService(uow)
                .BalanceComprobacionReport(new BasicReportParam { CompanyId = "C001" })
                .ToList();

            var caja = report.Single(r => r.Account == "Caja");
            Assert.Equal(100, caja.SaldoAnteriorDeb);
            Assert.Equal(0, caja.SaldoAnteriorCred);
            Assert.Equal(30, caja.SaldoMensualDeb);
            Assert.Equal(0, caja.SaldoMensualCred);
            Assert.Equal(130, caja.SaldoActualCuentaDeb);
            Assert.Equal(0, caja.SaldoActualCuentaCred);

            var ingresos = report.Single(r => r.Account == "Ingresos");
            Assert.Equal(0, ingresos.SaldoAnteriorDeb);
            Assert.Equal(200, ingresos.SaldoAnteriorCred);
            Assert.Equal(0, ingresos.SaldoMensualDeb);
            Assert.Equal(45, ingresos.SaldoMensualCred);
            Assert.Equal(0, ingresos.SaldoActualCuentaDeb);
            Assert.Equal(245, ingresos.SaldoActualCuentaCred);
        }

        [Fact]
        public void EstadoResultadoIntegral_skips_editable_auxiliar_with_zero_balance()
        {
            var uow = new FakeUnitOfWork();
            uow.FinancialReports.EstadoResultadoAccounts.AddRange(new[]
            {
                Titulo("Ingreso", AccountTag.Ingreso, 1000),
                Titulo("Costo venta", AccountTag.CostoVenta, 200),
                Titulo("Egreso", AccountTag.Egreso, 50),
                new Account
                {
                    Name = "Aux sin movimiento",
                    PathDirection = "Ingreso¡Aux",
                    AccountType = AccountType.Cuenta_Auxiliar,
                    AccountTag = AccountTag.Ingreso,
                    Editable = true,
                    DebOCred = DebOrCred.Credito
                },
                new Account
                {
                    Name = "Aux con saldo",
                    PathDirection = "Ingreso¡Ventas",
                    AccountType = AccountType.Cuenta_Auxiliar,
                    AccountTag = AccountTag.Ingreso,
                    Editable = true,
                    DebOCred = DebOrCred.Credito,
                    CreditBalance = 80
                }
            });

            var result = new FinancialReportService(uow)
                .EstadoResultadoIntegral(new BasicReportParam { CompanyId = "C001" });

            Assert.DoesNotContain(result.Results, r => r.AccountPath.EndsWith("Aux"));
            Assert.Contains(result.Results, r => r.AccountPath.EndsWith("Ventas"));
            Assert.Contains(result.Results, r => r.IsMainAccount && r.AccountPath == "Ingreso");
            Assert.Equal(750m, result.TotalPeridaGanancia);
        }

        [Fact]
        public void PreviousClosurePostingPeriodBalance_is_eri_total()
        {
            var uow = new FakeUnitOfWork();
            uow.FinancialReports.EstadoResultadoAccounts.AddRange(new[]
            {
                Titulo("Ingreso", AccountTag.Ingreso, 400),
                Titulo("Costo venta", AccountTag.CostoVenta, 100),
                Titulo("Egreso", AccountTag.Egreso, 25)
            });

            var balance = new FinancialReportService(uow)
                .PreviousClosurePostingPeriodBalance(new BasicReportParam { CompanyId = "C001" });

            Assert.Equal(275m, balance.Amount);
        }

        [Fact]
        public void AccountMoving_points_to_classic_excel_path()
        {
            var uow = new FakeUnitOfWork();
            var svc = new FinancialReportService(uow);
            Action call = () => { svc.AccountMoving(); };

            var ex = Assert.Throws<InvalidOperationException>(call);
            Assert.Contains("CuentaCL.GetInfoCompleta", ex.Message);
        }

        [Fact]
        public void JournalEntryReport_is_pass_through()
        {
            var uow = new FakeUnitOfWork();
            var row = new JournalEntryReport { JournalEntryNumber = 3, DebitAmount = 10, CreditAmount = 10 };
            uow.FinancialReports.JournalReports.Add(row);

            var output = new FinancialReportService(uow)
                .JournalEntryReport(new BasicReportParam { CompanyId = "C001" });

            Assert.Same(row, Assert.Single(output));
        }

        private static Account Titulo(string path, AccountTag tag, decimal currentAsCredit)
        {
            return new Account
            {
                PathDirection = path,
                AccountType = AccountType.Cuenta_Titulo,
                AccountTag = tag,
                DebOCred = DebOrCred.Credito,
                CreditBalance = currentAsCredit
            };
        }
    }
}
