namespace AriesContador.Data.Migrations
{
    /// <summary>
    /// Contrato de esquema que el código Dapper / SPs espera.
    /// Las pruebas de integración lo contrastan con information_schema.
    /// </summary>
    public static class ExpectedSchema
    {
        public static readonly string[] Tables =
        {
            "users",
            "companies",
            "accounts",
            "accounts_names",
            "accounting_months",
            "accounting_entries",
            "transactions_accounting",
            "posting_period_end_closing",
            "modules",
            "windows",
            "windows_permission",
            "companies_permission",
            "usuarios_correo",
            DatabaseMigrator.HistoryTableName
        };

        public static readonly string[] Views =
        {
            "account_info",
            "accounting_entries_info"
        };

        /// <summary>
        /// Vistas que crean las migraciones. <c>accounting_entries_info</c> solo existe en el dump.
        /// </summary>
        public static readonly string[] ViewsCreatedByMigrations =
        {
            "account_info"
        };

        public static readonly string[] Functions =
        {
            "F_GetAccountPathForReport",
            "GETFULLPATH",
            "GetAccountName"
        };

        /// <summary>
        /// SPs que llama AriesContador.Data (typos del dump incluidos).
        /// </summary>
        public static readonly string[] ProceduresCalledByCode =
        {
            "SP_InsertCompany",
            "SP_UpdateCompany",
            "SP_InsertUser",
            "SP_UpdateUser",
            "SP_GetAllUsers",
            "SP_FindUserById",
            "SP_InsertAccount",
            "SP_InsertChildAccount",
            "SP_GetAccountsByCompanyId",
            "SP_GetAccountById",
            "SP_DesactivateAccount",
            "SP_UpdateAccountNameInfo",
            "SP_AccountNameTaken",
            "SP_AccountHasOpenPeriodMovements",
            "SP_GetAccountBalancesFromAccountInfo",
            "SP_AuxiliaryAccountsWithBalanceByDateRange",
            "SP_GetOrCreateAccountName",
            "SP_InsertPostingPeriod",
            "SP_GetAllPostingPeriod",
            "SP_ClosePeriod",
            "SP_InsertClosingPostingPeriod",
            "SP_GetPostingPeriodReport",
            "SP_GetClosingPostingPeriodReport",
            "SP_InsertJournalEntry",
            "SP_UpdateJournalEntry",
            "SP_DesactivateJournalEntry",
            "SP_RestoreJournalEntry",
            "SP_GetJournalEntryById",
            "SP_GetJournalEntryByPostingPeriodId",
            "SP_GetJournalEntryConsecutive",
            "SP_GetJournalEntryDeletedBydDateRange",
            "SP_InsertJournalEntryLine",
            "SP_UpdateJournalEntryLine",
            "SP_DesactivateJournalEntryLine",
            "SP_RestoreJournalEntryLine",
            "SP_GetJournalEntryLineById",
            "SP_GetJournalEntryLineByJournalEntryId",
            "SP_GetAllJournalEntyLineByAccoudIdAndPostingPeriodId",
            "SP_GetJournalEntyLineDeletedByDateRange",
            "SP_JournalEntryReportByDateRange",
            "SP_EstadoResultadoIntegralReport"
        };

        /// <summary>
        /// SPs que recrean las migraciones. El resto de <see cref="ProceduresCalledByCode"/> vive en el dump.
        /// </summary>
        public static readonly string[] ProceduresCreatedByMigrations =
        {
            "SP_InsertCompany",
            "SP_UpdateCompany",
            "SP_InsertUser",
            "SP_UpdateUser",
            "SP_InsertChildAccount",
            "SP_AccountHasOpenPeriodMovements",
            "SP_GetAccountBalancesFromAccountInfo",
            "SP_InsertAccount",
            "SP_GetAccountsByCompanyId",
            "SP_GetAllPostingPeriod",
            "SP_GetClosingPostingPeriodReport",
            "SP_GetPostingPeriodReport",
            "SP_InsertClosingPostingPeriod",
            "SP_InsertPostingPeriod"
        };

        public static readonly ExpectedColumn[] Columns =
        {
            new ExpectedColumn("users", "user_id"),
            new ExpectedColumn("users", "user_name"),
            new ExpectedColumn("users", "password") { MinCharLength = 255, Nullable = false },
            new ExpectedColumn("users", "user_type"),
            new ExpectedColumn("users", "active"),
            new ExpectedColumn("companies", "company_id") { CharLength = 5, Nullable = false },
            new ExpectedColumn("companies", "user_id") { Nullable = false },
            new ExpectedColumn("companies", "number_id"),
            new ExpectedColumn("companies", "name"),
            new ExpectedColumn("companies", "money_type"),
            new ExpectedColumn("companies", "active"),
            new ExpectedColumn("accounts", "account_id"),
            new ExpectedColumn("accounts", "account_name_id"),
            new ExpectedColumn("accounts", "father_account"),
            new ExpectedColumn("accounts", "company_id") { CharLength = 5, Nullable = false },
            new ExpectedColumn("accounts", "account_type"),
            new ExpectedColumn("accounts", "account_guide"),
            new ExpectedColumn("accounts_names", "account_name_id"),
            new ExpectedColumn("accounts_names", "name"),
            new ExpectedColumn("accounting_months", "accounting_months_id"),
            new ExpectedColumn("accounting_months", "month_report"),
            new ExpectedColumn("accounting_months", "closed"),
            new ExpectedColumn("accounting_months", "company_id") { CharLength = 5, Nullable = false },
            new ExpectedColumn("accounting_entries", "accounting_entry_id"),
            new ExpectedColumn("accounting_entries", "entry_id"),
            new ExpectedColumn("accounting_entries", "accounting_months_id"),
            new ExpectedColumn("accounting_entries", "status"),
            new ExpectedColumn("accounting_entries", "active"),
            new ExpectedColumn("transactions_accounting", "transaction_accounting_id"),
            new ExpectedColumn("transactions_accounting", "account_id"),
            new ExpectedColumn("transactions_accounting", "accounting_entry_id"),
            new ExpectedColumn("transactions_accounting", "balance"),
            new ExpectedColumn("transactions_accounting", "balance_type"),
            new ExpectedColumn("posting_period_end_closing", "Id"),
            new ExpectedColumn("posting_period_end_closing", "company_id") { CharLength = 5, Nullable = false },
            new ExpectedColumn("modules", "module_id"),
            new ExpectedColumn("modules", "internal_name"),
            new ExpectedColumn("windows", "window_id"),
            new ExpectedColumn("windows", "internal_name"),
            new ExpectedColumn("windows", "comments"),
            new ExpectedColumn("windows_permission", "user_id"),
            new ExpectedColumn("windows_permission", "window_id"),
            new ExpectedColumn("companies_permission", "user_id"),
            new ExpectedColumn("companies_permission", "company_id") { CharLength = 5, Nullable = false },
            new ExpectedColumn("companies_permission", "deleted"),
            new ExpectedColumn("usuarios_correo", "mailusuario_id"),
            new ExpectedColumn("usuarios_correo", "correo_electronico"),
            new ExpectedColumn(DatabaseMigrator.HistoryTableName, "migration_id"),
            new ExpectedColumn(DatabaseMigrator.HistoryTableName, "version")
        };

        public static readonly ExpectedParameter[] ProcedureParameters =
        {
            new ExpectedParameter("SP_InsertCompany", "NewCompanyId", "varchar(5)", "OUT"),
            new ExpectedParameter("SP_InsertUser", "Password", "varchar(255)", "IN"),
            new ExpectedParameter("SP_InsertUser", "Id", "int", "OUT"),
            new ExpectedParameter("SP_UpdateUser", "Password", "varchar(255)", "IN"),
            new ExpectedParameter("SP_InsertChildAccount", "Id", "int", "OUT"),
            new ExpectedParameter("SP_InsertChildAccount", "CompanyId", "varchar(5)", "IN"),
            new ExpectedParameter("SP_InsertAccount", "CompanyId", "varchar(5)", "IN"),
            new ExpectedParameter("SP_InsertAccount", "Id", "int", "OUT"),
            new ExpectedParameter("SP_GetOrCreateAccountName", "Id", "int", "OUT"),
            new ExpectedParameter("SP_GetAccountsByCompanyId", "CompanyId", "varchar(5)", "IN"),
            new ExpectedParameter("SP_GetAllPostingPeriod", "CompanyId", "varchar(5)", "IN"),
            new ExpectedParameter("SP_InsertPostingPeriod", "CompanyId", "varchar(5)", "IN"),
            new ExpectedParameter("SP_InsertClosingPostingPeriod", "CompanyId", "varchar(5)", "IN"),
            new ExpectedParameter("SP_GetPostingPeriodReport", "CompanyId", "varchar(5)", "IN"),
            new ExpectedParameter("SP_GetClosingPostingPeriodReport", "CompanyId", "varchar(5)", "IN")
        };

        public static readonly ExpectedForeignKey[] PermissionForeignKeys =
        {
            new ExpectedForeignKey("companies_permission", "fk_companies_permission_user", "users"),
            new ExpectedForeignKey("companies_permission", "fk_companies_permission_company", "companies"),
            new ExpectedForeignKey("windows_permission", "fk_windows_permission_user", "users"),
            new ExpectedForeignKey("windows_permission", "fk_windows_permission_module", "modules"),
            new ExpectedForeignKey("windows_permission", "fk_windows_permission_window", "windows")
        };

        public const string UniqueCompanyMonthIndex = "uk_accounting_months_company_month";
    }

    public sealed class ExpectedColumn
    {
        public ExpectedColumn(string table, string name)
        {
            Table = table;
            Name = name;
        }

        public string Table { get; }
        public string Name { get; }
        public int? CharLength { get; set; }
        public int? MinCharLength { get; set; }
        public bool? Nullable { get; set; }
    }

    public sealed class ExpectedParameter
    {
        public ExpectedParameter(string routine, string name, string typeContains, string mode)
        {
            Routine = routine;
            Name = name;
            TypeContains = typeContains;
            Mode = mode;
        }

        public string Routine { get; }
        public string Name { get; }
        public string TypeContains { get; }
        public string Mode { get; }
    }

    public sealed class ExpectedForeignKey
    {
        public ExpectedForeignKey(string table, string constraint, string referencedTable)
        {
            Table = table;
            Constraint = constraint;
            ReferencedTable = referencedTable;
        }

        public string Table { get; }
        public string Constraint { get; }
        public string ReferencedTable { get; }
    }
}
