-- Correr en Workbench contra RDS `aries` (prod).
-- El 401 de http://arieswebapi7-dev.../auth/login NO sale de un SP nuevo.
-- El exe 1.1.18 llama al API de Elastic Beanstalk; ese API hace SP_GetAllUsers
-- y compara password en texto plano. Si el exe nuevo (1.2) ya inició sesión,
-- reescribió users.password a pbkdf2$... y el API responde 401.
--
-- 1) Diagnóstico. Si hashed = 1, ese usuario no puede entrar por 1.1.18 / EBS.

SELECT
    user_id,
    user_name,
    active,
    user_type,
    password LIKE 'pbkdf2$%' AS hashed,
    CHAR_LENGTH(password) AS pwd_len,
    LEFT(password, 30) AS pwd_prefix
FROM users
ORDER BY user_id;

SHOW CREATE PROCEDURE SP_GetAllUsers;

-- 2) Arreglo: volver a clave plana. El hash no se puede revertir.
--    Poner la contraseña que esa persona usaba ANTES del exe nuevo.
--    Repetir un UPDATE por cada fila con hashed = 1.

-- UPDATE users
-- SET password = 'CLAVE_PLANA_AQUI', updated_at = NOW()
-- WHERE user_name = 'USUARIO_AQUI'
--   AND password LIKE 'pbkdf2$%';

-- 3) Comprobar que ya no quedan hashes:

SELECT user_name, password LIKE 'pbkdf2$%' AS hashed
FROM users
WHERE active = 1;
