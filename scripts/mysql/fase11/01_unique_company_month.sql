-- Apply only on a copy of `aries` after 00_inventory_duplicate_months.sql is empty.
-- Dump local 2026-09: C001 tiene duplicados en 2021-01, 2022-10, 2022-11, 2022-12.
-- NO correr este ALTER hasta resolver esas filas.

ALTER TABLE `accounting_months`
    ADD UNIQUE KEY `uk_accounting_months_company_month` (`company_id`, `month_report`);
