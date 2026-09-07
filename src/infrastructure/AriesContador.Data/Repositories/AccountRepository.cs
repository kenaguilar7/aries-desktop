using AriesContador.Core.Models.Accounts;
using AriesContador.Core.Models.JournalEntries;
using AriesContador.Core.Models.Utils;
using AriesContador.Core.Repositories;
using AriesContador.Data.Internal.DataAccess;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AriesContador.Data.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private readonly IConnectionString _connectionString;
        public AccountRepository(IConnectionString connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task AddAsync(Account entity, CancellationToken cancellationToken = default)
        {
            var dataAccess = new MySqlDataAccess(_connectionString);
            entity.Id = await dataAccess.SaveDataAsync<Account, int>("SP_InsertAccount", entity, cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task AddChildAsync(Account entity, CancellationToken cancellationToken = default)
        {
            var dataAccess = new MySqlDataAccess(_connectionString);
            entity.Id = await dataAccess.SaveDataAsync<object, int>("SP_InsertChildAccount", ToChildInsertParams(entity), cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task<IEnumerable<Account>> FindByCompanyIdAsync(string companyId, CancellationToken cancellationToken = default)
        {
            var dataAccess = new MySqlDataAccess(_connectionString);
            return await dataAccess.LoadDataAsync<Account, dynamic>("SP_GetAccountsByCompanyId", new { CompanyId = companyId }, cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task<Account> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var dataAccess = new MySqlDataAccess(_connectionString);
            var output = await dataAccess.LoadDataAsync<Account, dynamic>("SP_GetAccountById", new { accountId = id }, cancellationToken)
                .ConfigureAwait(false);
            return output.FirstOrDefault();
        }

        public async Task RemoveAsync(Account entity, CancellationToken cancellationToken = default)
        {
            var dataAccess = new MySqlDataAccess(_connectionString);
            await dataAccess.SaveDataAsync("SP_DesactivateAccount", entity, cancellationToken).ConfigureAwait(false);
        }

        public Task UpdateAsync(Account entity, CancellationToken cancellationToken = default)
        {
            return UpdateNameInfoAsync(entity, cancellationToken);
        }

        public async Task UpdateNameInfoAsync(Account entity, CancellationToken cancellationToken = default)
        {
            var dataAccess = new MySqlDataAccess(_connectionString);
            await dataAccess.SaveDataAsync("SP_UpdateAccountNameInfo", new
            {
                entity.Id,
                entity.Name,
                entity.Memo,
                entity.CompanyId,
                entity.UpdatedBy
            }, cancellationToken).ConfigureAwait(false);
        }

        public async Task<bool> NameTakenAsync(int accountId, string companyId, string name, CancellationToken cancellationToken = default)
        {
            var dataAccess = new MySqlDataAccess(_connectionString);
            var rows = await dataAccess.LoadDataAsync<FlagRow, dynamic>("SP_AccountNameTaken", new
            {
                AccountId = accountId,
                CompanyId = companyId,
                Name = name
            }, cancellationToken).ConfigureAwait(false);
            return rows.FirstOrDefault()?.Taken == 1;
        }

        public async Task<bool> HasOpenPeriodMovementsAsync(int accountId, CancellationToken cancellationToken = default)
        {
            var dataAccess = new MySqlDataAccess(_connectionString);
            var rows = await dataAccess.LoadDataAsync<FlagRow, dynamic>("SP_AccountHasOpenPeriodMovements", new
            {
                AccountId = accountId
            }, cancellationToken).ConfigureAwait(false);
            return rows.FirstOrDefault()?.HasMovements == 1;
        }

        public async Task<IEnumerable<Account>> GetBalancesFromAccountInfoAsync(string companyId, DateTime from, DateTime to, CancellationToken cancellationToken = default)
        {
            var dataAccess = new MySqlDataAccess(_connectionString);
            return await dataAccess.LoadDataAsync<Account, dynamic>("SP_GetAccountBalancesFromAccountInfo", new
            {
                CompanyId = companyId,
                FromPeriod = AccountRules.ToYearMonthKey(from),
                ToPeriod = AccountRules.ToYearMonthKey(to)
            }, cancellationToken).ConfigureAwait(false);
        }

        public IEnumerable<Account> GetDefaultAccounts()
        {
            return DefaultChartOfAccounts.Create();
        }

        public async Task<IEnumerable<Account>> AccountsWithBalanceByDateRangeAsync(BasicReportParam reportParam, CancellationToken cancellationToken = default)
        {
            var dataAccess = new MySqlDataAccess(_connectionString);
            var output = await dataAccess.LoadDataAsync<Account, BasicReportParam>("SP_AuxiliaryAccountsWithBalanceByDateRange", reportParam, cancellationToken)
                .ConfigureAwait(false);
            output.BuildAccountsBalance();
            return output.OrderByTree();
        }

        public async Task<DataTable> GetMovementReportAsync(int accountId, bool auxiliar, CancellationToken cancellationToken = default)
        {
            var filter = auxiliar
                ? "T3.account_id = @AccountId"
                : "T3.father_account = @AccountId";
            var tipo = auxiliar ? "Movimiento a cuenta" : "Movimiento a hija";
            var sql = "SELECT "
                      + "(SELECT T1.name FROM accounts_names T1 where T1.account_name_id = T3.account_name_id LIMIT 1) AS 'Nombre', "
                      + $"IF(T3.account_guide <> 'CUENTA AUXILIAR', 'Movimiento a hija', '{tipo}' ) AS 'Tipo Moviento',"
                      + "T2.detail AS 'Detalle',"
                      + "T2.reference AS 'Referencia', "
                      + "T2.bill_date AS 'Fecha Documento', "
                      + "DATE_FORMAT(T5.month_report,'%M %Y') AS 'Mes Contable', "
                      + "T4.entry_id 'Numero de Asiento',"
                      + "IF(T2.balance_type+0 = 1,FORMAT(T2.balance,2), null) AS 'Debito', "
                      + "IF(T2.balance_type+0 = 2,FORMAT(T2.balance,2), null) AS 'Credito', "
                      + "FORMAT(T3.account_type+0,0) AS 'Saldo Actual',"
                      + "FORMAT(T2.money_chance,2) AS 'Tipo Cambio',"
                      + "IF(T2.money_type+0 = 2, FORMAT(T2.balance/T2.money_chance,2), FORMAT(0.00,2)) AS 'Monto Dolares',  "
                      + "(SELECT T7.user_name FROM users T7 WHERE T7.user_id = T2.updated_by LIMIT 1)  AS 'Usuario registro', "
                      + "T2.created_at AS 'Fecha de Registro' "
                      + "FROM transactions_accounting T2 LEFT JOIN accounts T3 ON T2.account_id = T3.account_id "
                      + "LEFT JOIN accounting_entries T4 ON T2.accounting_entry_id = T4.accounting_entry_id "
                      + "LEFT JOIN accounting_months T5 USING(accounting_months_id) "
                      + $"WHERE {filter} AND T2.active = 1 AND T3.active = 1 AND T4.active = 1 "
                      + "ORDER BY T5.month_report, T4.entry_id";
            var dataAccess = new MySqlDataAccess(_connectionString);
            return await dataAccess.QueryTableAsync(sql, new { AccountId = accountId }, cancellationToken)
                .ConfigureAwait(false);
        }

        private static object ToChildInsertParams(Account entity)
        {
            return new
            {
                entity.Name,
                entity.PriorBalance,
                entity.PriorBalanceForeign,
                FatherAccount = entity.FatherAccount ?? 0,
                entity.CompanyId,
                AccountType = (int)entity.AccountType,
                AccountTag = (int)entity.AccountTag,
                entity.Memo,
                Editable = entity.EditableMySql,
                entity.UpdatedBy
            };
        }

        private class FlagRow
        {
            public int Taken { get; set; }
            public int HasMovements { get; set; }
        }
    }
}
