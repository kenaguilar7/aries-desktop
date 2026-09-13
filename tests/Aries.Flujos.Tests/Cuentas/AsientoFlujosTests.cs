using System.Linq;
using System.Threading.Tasks;
using Aries.Flujos.Tests.Infrastructure;
using AriesContador.Core.Models.JournalEntries;
using AriesContador.Core.Models.Utils;
using Xunit;

namespace Aries.Flujos.Tests.Cuentas
{
    [Collection("flujos-db")]
    public class AsientoFlujosTests
    {
        private readonly FlujosDbFixture _db;

        public AsientoFlujosTests(FlujosDbFixture db)
        {
            _db = db;
        }

        [DockerFact]
        public async Task Crear_asiento_3_hope_inserta_header_y_dos_lineas_cuadradas()
        {
            var ctx = await HopeCuentasContexto.CrearAsync(_db.Services);
            var financial = _db.Services.Financial;
            var entry = await ctx.CrearAsiento3Async(financial);

            Assert.True(entry.Id > 0);
            Assert.Equal(1, entry.Number);
            Assert.Equal(JournalEntryStatus.Approved, entry.JournalEntryStatus);
            Assert.True(entry.Cuadrado);

            var headerActive = FlujosSql.Scalar<int>(
                _db.ConnectionString,
                "SELECT active FROM accounting_entries WHERE accounting_entry_id = @id",
                ("@id", entry.Id));
            Assert.Equal(1, headerActive);

            var lineCount = FlujosSql.Scalar<long>(
                _db.ConnectionString,
                "SELECT COUNT(*) FROM transactions_accounting WHERE accounting_entry_id = @id AND active = 1",
                ("@id", entry.Id));
            Assert.Equal(2L, lineCount);

            var debito = FlujosSql.Scalar<decimal>(
                _db.ConnectionString,
                @"SELECT SUM(balance) FROM transactions_accounting
                  WHERE accounting_entry_id = @id AND active = 1 AND balance_type + 0 = 1",
                ("@id", entry.Id));
            var credito = FlujosSql.Scalar<decimal>(
                _db.ConnectionString,
                @"SELECT SUM(balance) FROM transactions_accounting
                  WHERE accounting_entry_id = @id AND active = 1 AND balance_type + 0 = 2",
                ("@id", entry.Id));
            Assert.Equal(HopeMuestra.Asiento3DatafonoDebito, debito);
            Assert.Equal(HopeMuestra.Asiento3CirugiasCredito, credito);

            var lines = (await financial.GetJournalEntryLineByJournalEntryIdAsync(entry.Id)).ToList();
            Assert.Equal(2, lines.Count);
            Assert.Contains(lines, l => l.AccountId == ctx.UsoDatafono.Id && l.DebOrCred == DebOrCred.Debito);
            Assert.Contains(lines, l => l.AccountId == ctx.IngresosCirugias.Id && l.DebOrCred == DebOrCred.Credito);
            Assert.All(lines, l =>
            {
                Assert.Equal(HopeMuestra.RefAsiento3, l.Reference);
                Assert.Equal(HopeMuestra.DetalleAsiento3, l.Memo);
            });
        }

        [DockerFact]
        public async Task Crear_asiento_2_hope_queda_cuadrado_con_cuatro_lineas()
        {
            var ctx = await HopeCuentasContexto.CrearAsync(_db.Services);
            var entry = await ctx.CrearAsiento2Async(_db.Services.Financial);

            Assert.True(entry.Cuadrado);
            Assert.Equal(HopeMuestra.Asiento2CxcCredito, entry.CreditosColones);
            Assert.Equal(HopeMuestra.Asiento2CxcCredito, entry.DebitosColones);

            var lineCount = FlujosSql.Scalar<long>(
                _db.ConnectionString,
                "SELECT COUNT(*) FROM transactions_accounting WHERE accounting_entry_id = @id AND active = 1",
                ("@id", entry.Id));
            Assert.Equal(4L, lineCount);
        }

        [DockerFact]
        public async Task Editar_linea_persiste_referencia_y_mantiene_el_asiento()
        {
            var ctx = await HopeCuentasContexto.CrearAsync(_db.Services);
            var financial = _db.Services.Financial;
            var entry = await ctx.CrearAsiento3Async(financial);
            var debit = (await financial.GetJournalEntryLineByJournalEntryIdAsync(entry.Id))
                .Single(l => l.DebOrCred == DebOrCred.Debito);

            debit.Reference = "ajuste ventas periodo - editado";
            debit.Memo = "ajuste ventas periodo HOPE";
            debit.UpdatedBy = FlujosDbFixture.AdminUserId;
            await financial.UpdateJournalEntryLineAsync(debit);

            var reference = FlujosSql.Scalar<string>(
                _db.ConnectionString,
                "SELECT reference FROM transactions_accounting WHERE transaction_accounting_id = @id",
                ("@id", debit.Id));
            Assert.Equal("ajuste ventas periodo - editado", reference);

            var memo = FlujosSql.Scalar<string>(
                _db.ConnectionString,
                "SELECT detail FROM transactions_accounting WHERE transaction_accounting_id = @id",
                ("@id", debit.Id));
            Assert.Equal("ajuste ventas periodo HOPE", memo);
        }

        [DockerFact]
        public async Task Eliminar_linea_la_desactiva_sin_borrar_el_asiento()
        {
            var ctx = await HopeCuentasContexto.CrearAsync(_db.Services);
            var financial = _db.Services.Financial;
            var entry = await ctx.CrearAsiento3Async(financial);
            var credit = (await financial.GetJournalEntryLineByJournalEntryIdAsync(entry.Id))
                .Single(l => l.DebOrCred == DebOrCred.Credito);

            credit.UpdatedBy = FlujosDbFixture.AdminUserId;
            await financial.DeleteJournalEntryLineAsync(credit);

            var lineActive = FlujosSql.Scalar<int>(
                _db.ConnectionString,
                "SELECT active FROM transactions_accounting WHERE transaction_accounting_id = @id",
                ("@id", credit.Id));
            Assert.Equal(0, lineActive);

            var remaining = (await financial.GetJournalEntryLineByJournalEntryIdAsync(entry.Id)).ToList();
            Assert.Single(remaining);
            Assert.Equal(DebOrCred.Debito, remaining[0].DebOrCred);

            var headerActive = FlujosSql.Scalar<int>(
                _db.ConnectionString,
                "SELECT active FROM accounting_entries WHERE accounting_entry_id = @id",
                ("@id", entry.Id));
            Assert.Equal(1, headerActive);
        }

        [DockerFact]
        public async Task Eliminar_asiento_desactiva_el_header()
        {
            var ctx = await HopeCuentasContexto.CrearAsync(_db.Services);
            var financial = _db.Services.Financial;
            var entry = await ctx.CrearAsiento3Async(financial);

            entry.UpdatedBy = FlujosDbFixture.AdminUserId;
            await financial.DeleteJournalEntryAsync(entry);

            var headerActive = FlujosSql.Scalar<int>(
                _db.ConnectionString,
                "SELECT active FROM accounting_entries WHERE accounting_entry_id = @id",
                ("@id", entry.Id));
            Assert.Equal(0, headerActive);

            var listed = await financial.GetJournalEntriesAsync(ctx.Periodo.Id);
            Assert.DoesNotContain(listed, e => e.Id == entry.Id);
        }

        [DockerFact]
        public async Task Agregar_linea_por_servicio_a_asiento_ya_guardado()
        {
            var ctx = await HopeCuentasContexto.CrearAsync(_db.Services);
            var financial = _db.Services.Financial;
            var number = await financial.CreateJournalEntryConsecutiveAsync(ctx.Periodo.Id);
            var entry = new JournalEntry
            {
                Number = number,
                PostingPeriodId = ctx.Periodo.Id,
                JournalEntryStatus = JournalEntryStatus.Progress,
                UpdatedBy = FlujosDbFixture.AdminUserId,
                CreatedBy = FlujosDbFixture.AdminUserId,
                Active = true
            };
            await financial.CreateJournalEntryAsync(entry);

            var debit = HopeCuentasContexto.Linea(
                ctx.UsoDatafono.Id,
                HopeMuestra.CuentaUsoDatafono,
                HopeMuestra.RefAsiento3,
                HopeMuestra.DetalleAsiento3,
                HopeMuestra.FechaAsiento3,
                HopeMuestra.Asiento3DatafonoDebito,
                DebOrCred.Debito);
            debit.JournalEntryId = entry.Id;
            await financial.CreateJournalEntryLineAsync(debit);

            var credit = HopeCuentasContexto.Linea(
                ctx.IngresosCirugias.Id,
                HopeMuestra.CuentaIngresosCirugias,
                HopeMuestra.RefAsiento3,
                HopeMuestra.DetalleAsiento3,
                HopeMuestra.FechaAsiento3,
                HopeMuestra.Asiento3CirugiasCredito,
                DebOrCred.Credito);
            credit.JournalEntryId = entry.Id;
            await financial.CreateJournalEntryLineAsync(credit);

            entry.JournalEntryLines.Add(debit);
            entry.JournalEntryLines.Add(credit);
            entry.ApplyStatusFromBalance();
            await financial.UpdateJournalEntryAsync(entry);

            Assert.True(debit.Id > 0);
            Assert.True(credit.Id > 0);
            var status = FlujosSql.Scalar<int>(
                _db.ConnectionString,
                "SELECT status + 0 FROM accounting_entries WHERE accounting_entry_id = @id",
                ("@id", entry.Id));
            Assert.Equal((int)JournalEntryStatus.Approved, status);
        }
    }
}
