-- Apply only on a copy of `aries` (Docker :3307), never on RDS production.
-- Run BEFORE 01_unique_company_month.sql. If this returns rows, resolve duplicates first.

SELECT company_id, month_report, COUNT(*) AS dupes
FROM accounting_months
GROUP BY company_id, month_report
HAVING COUNT(*) > 1;
