-- Apply only on a copy of `aries` after 03_cleanup_permission_orphans.sql.

ALTER TABLE `companies_permission`
    ADD CONSTRAINT `fk_companies_permission_user`
        FOREIGN KEY (`user_id`) REFERENCES `users` (`user_id`),
    ADD CONSTRAINT `fk_companies_permission_company`
        FOREIGN KEY (`company_id`) REFERENCES `companies` (`company_id`);

ALTER TABLE `windows_permission`
    ADD CONSTRAINT `fk_windows_permission_user`
        FOREIGN KEY (`user_id`) REFERENCES `users` (`user_id`),
    ADD CONSTRAINT `fk_windows_permission_module`
        FOREIGN KEY (`module_id`) REFERENCES `modules` (`module_id`),
    ADD CONSTRAINT `fk_windows_permission_window`
        FOREIGN KEY (`window_id`) REFERENCES `windows` (`window_id`);
