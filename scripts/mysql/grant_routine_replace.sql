-- Permite al usuario de app reemplazar SPs/funciones restaurados por root (dump).
-- MySQL 8.0: DROP de rutinas creadas por un SYSTEM_USER exige este privilegio.
-- Idempotente. start-local.ps1 lo aplica en cada arranque.

GRANT SYSTEM_USER ON *.* TO 'kenneth'@'%';
FLUSH PRIVILEGES;
