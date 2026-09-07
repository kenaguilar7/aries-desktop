-- Apply only on a copy of `aries`.
-- 1) Inspect: SELECT company_id FROM companies WHERE user_id IS NULL;
-- 2) Assign an admin user_id to those rows, then run this.

-- UPDATE companies SET user_id = (SELECT user_id FROM users WHERE user_type = 'Administrador' LIMIT 1)
-- WHERE user_id IS NULL;

ALTER TABLE `companies`
    MODIFY COLUMN `user_id` INT UNSIGNED NOT NULL;
