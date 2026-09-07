-- Apply only on a copy of `aries` BEFORE adding FKs.

DELETE cp FROM companies_permission cp
LEFT JOIN users u ON u.user_id = cp.user_id
WHERE u.user_id IS NULL;

DELETE cp FROM companies_permission cp
LEFT JOIN companies c ON c.company_id = cp.company_id
WHERE c.company_id IS NULL;

DELETE wp FROM windows_permission wp
LEFT JOIN users u ON u.user_id = wp.user_id
WHERE u.user_id IS NULL;

DELETE wp FROM windows_permission wp
LEFT JOIN modules m ON m.module_id = wp.module_id
WHERE m.module_id IS NULL;

DELETE wp FROM windows_permission wp
LEFT JOIN windows w ON w.window_id = wp.window_id
WHERE w.window_id IS NULL;
