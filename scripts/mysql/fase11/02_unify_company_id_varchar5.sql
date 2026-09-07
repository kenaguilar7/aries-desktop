-- Apply only on a copy of `aries`. Unifies company_id to varchar(5) so C1000 fits.
-- Measure first: SELECT MAX(CHAR_LENGTH(company_id)) FROM companies;

ALTER TABLE `accounts`
    MODIFY COLUMN `company_id` VARCHAR(5) NOT NULL;

ALTER TABLE `accounting_months`
    MODIFY COLUMN `company_id` VARCHAR(5) NOT NULL;

ALTER TABLE `companies_permission`
    MODIFY COLUMN `company_id` VARCHAR(5) NOT NULL;

ALTER TABLE `posting_period_end_closing`
    MODIFY COLUMN `company_id` VARCHAR(5) NOT NULL;
