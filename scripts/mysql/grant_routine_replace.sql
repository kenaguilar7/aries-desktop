-- Permite al usuario de app reemplazar SPs/funciones restaurados por root (dump).
-- MySQL 8.0: DROP de rutinas creadas por un SYSTEM_USER exige este privilegio.
-- CREATE FUNCTION con binary logging exige SUPER o log_bin_trust_function_creators.
-- Idempotente. start-local.ps1 lo aplica en cada arranque (como root).

SET GLOBAL log_bin_trust_function_creators = 1;
GRANT SYSTEM_USER ON *.* TO 'kenneth'@'%';
FLUSH PRIVILEGES;
