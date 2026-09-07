using System;
using AriesContador.Data.Migrations;
using Xunit;

namespace Aries.Data.Tests
{
    [Collection("mysql-schema")]
    public class DatabaseSchemaTests
    {
        private readonly DatabaseSchemaReader _schema = new DatabaseSchemaReader(MySqlTestConnection.ConnectionString);

        [MySqlFact]
        public void Required_tables_exist()
        {
            var tables = _schema.TableNames();
            foreach (var table in ExpectedSchema.Tables)
                Assert.Contains(table, tables);
        }

        [MySqlFact]
        public void Required_views_exist()
        {
            var views = _schema.ViewNames();
            foreach (var view in ExpectedSchema.ViewsCreatedByMigrations)
                Assert.Contains(view, views);

            if (HasDumpRoutines())
            {
                foreach (var view in ExpectedSchema.Views)
                    Assert.Contains(view, views);
            }
        }

        [MySqlFact]
        public void Required_functions_exist()
        {
            var functions = _schema.FunctionNames();
            foreach (var function in ExpectedSchema.Functions)
                Assert.Contains(function, functions);
        }

        [MySqlFact]
        public void Procedures_called_by_code_exist()
        {
            var procedures = _schema.ProcedureNames();
            var required = HasDumpRoutines()
                ? ExpectedSchema.ProceduresCalledByCode
                : ExpectedSchema.ProceduresCreatedByMigrations;
            foreach (var procedure in required)
                Assert.Contains(procedure, procedures);
        }

        [MySqlFact]
        public void Columns_match_code_contract()
        {
            foreach (var expected in ExpectedSchema.Columns)
            {
                Assert.True(_schema.ColumnExists(expected.Table, expected.Name), expected.Table + "." + expected.Name);

                var column = _schema.GetColumn(expected.Table, expected.Name);
                Assert.NotNull(column);

                if (expected.Nullable.HasValue)
                    Assert.Equal(expected.Nullable.Value, column.Nullable);

                if (expected.CharLength.HasValue)
                    Assert.Equal(expected.CharLength.Value, column.CharLength);

                if (expected.MinCharLength.HasValue)
                    Assert.True(
                        column.CharLength.GetValueOrDefault() >= expected.MinCharLength.Value,
                        expected.Table + "." + expected.Name + " length " + column.CharLength);
            }
        }

        [MySqlFact]
        public void Procedure_parameters_match_dapper_contract()
        {
            foreach (var expected in ExpectedSchema.ProcedureParameters)
            {
                var parameter = _schema.GetParameter(expected.Routine, expected.Name);
                Assert.NotNull(parameter);
                Assert.Equal(expected.Mode, parameter.Mode, StringComparer.OrdinalIgnoreCase);
                Assert.Contains(expected.TypeContains, parameter.Type, StringComparison.OrdinalIgnoreCase);
            }
        }

        [MySqlFact]
        public void Permission_foreign_keys_exist()
        {
            foreach (var fk in ExpectedSchema.PermissionForeignKeys)
                Assert.True(_schema.ForeignKeyExists(fk.Table, fk.Constraint), fk.Constraint);
        }

        [MySqlFact]
        public void Unique_company_month_exists_or_duplicates_remain()
        {
            var hasIndex = _schema.IndexExists("accounting_months", ExpectedSchema.UniqueCompanyMonthIndex);
            var dupes = _schema.DuplicateCompanyMonthCount();
            Assert.True(hasIndex || dupes > 0,
                "Sin duplicados debería existir uk_accounting_months_company_month");
        }

        private bool HasDumpRoutines()
        {
            return _schema.ProcedureNames().Contains("SP_GetJournalEntryById");
        }
    }
}
