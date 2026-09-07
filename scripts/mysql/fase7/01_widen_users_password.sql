-- Apply only on a copy of `aries` (Docker :3307), never on RDS production.
-- PBKDF2 hashes exceed varchar(50). Widen before enabling login rehash.

ALTER TABLE `users`
    MODIFY COLUMN `password` VARCHAR(255) NOT NULL;
