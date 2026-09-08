namespace AriesContador.Data.Migrations.Scripts
{
    /// <summary>
    /// Copia el plan de cuentas en bloque al crear compañía.
    /// Evita N×2 round-trips (GetOrCreateAccountName + InsertAccount).
    /// </summary>
    public sealed class M013_CopyCompanyChart : SqlMigration
    {
        public override int Version => 13;
        public override string Id => "013_CopyCompanyChart";
        public override string Description => "SP_CopyChartOfAccounts y SP_InsertChartFromTemp";

        public override string Sql => @"
DROP PROCEDURE IF EXISTS `SP_CopyChartOfAccounts`;
-- BATCH
CREATE PROCEDURE `SP_CopyChartOfAccounts`(
    IN FromCompany VARCHAR(5),
    IN ToCompany VARCHAR(5),
    IN UpdatedBy INT
)
BEGIN
    DECLARE src_count INT DEFAULT 0;

    DROP TEMPORARY TABLE IF EXISTS tmp_chart_src;
    DROP TEMPORARY TABLE IF EXISTS tmp_chart_src_father;
    DROP TEMPORARY TABLE IF EXISTS tmp_chart_new;
    DROP TEMPORARY TABLE IF EXISTS tmp_chart_new_parent;

    CREATE TEMPORARY TABLE tmp_chart_src (
        seq INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
        old_id INT UNSIGNED NOT NULL,
        father_old INT UNSIGNED NULL
    );

    INSERT INTO tmp_chart_src (old_id, father_old)
    SELECT a.account_id, a.father_account
    FROM accounts a
    WHERE a.company_id = FromCompany
      AND a.active = 1
    ORDER BY a.account_id;

    SELECT COUNT(*) INTO src_count FROM tmp_chart_src;
    IF src_count = 0 THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'No se pudo clonar el maestro de cuentas';
    END IF;

    INSERT INTO accounts (
        account_name_id, father_account, company_id, account_type, account_guide,
        editable, detail, created_at, updated_at, updated_by, active
    )
    SELECT
        a.account_name_id,
        NULL,
        ToCompany,
        a.account_type,
        a.account_guide,
        a.editable,
        a.detail,
        NOW(), NOW(),
        IF(UpdatedBy IS NULL OR UpdatedBy = 0, a.updated_by, UpdatedBy),
        1
    FROM tmp_chart_src s
    INNER JOIN accounts a ON a.account_id = s.old_id
    ORDER BY s.seq;

    CREATE TEMPORARY TABLE tmp_chart_new (
        seq INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
        new_id INT UNSIGNED NOT NULL
    );

    INSERT INTO tmp_chart_new (new_id)
    SELECT a.account_id
    FROM accounts a
    WHERE a.company_id = ToCompany
    ORDER BY a.account_id;

    CREATE TEMPORARY TABLE tmp_chart_src_father AS SELECT seq, old_id, father_old FROM tmp_chart_src;
    CREATE TEMPORARY TABLE tmp_chart_new_parent AS SELECT seq, new_id FROM tmp_chart_new;

    UPDATE accounts dest
    INNER JOIN tmp_chart_new n ON n.new_id = dest.account_id
    INNER JOIN tmp_chart_src s ON s.seq = n.seq
    LEFT JOIN tmp_chart_src_father p ON p.old_id = s.father_old
    LEFT JOIN tmp_chart_new_parent np ON np.seq = p.seq
    SET dest.father_account = np.new_id
    WHERE dest.company_id = ToCompany;
END
-- BATCH
DROP PROCEDURE IF EXISTS `SP_InsertChartFromTemp`;
-- BATCH
CREATE PROCEDURE `SP_InsertChartFromTemp`(
    IN ToCompany VARCHAR(5)
)
BEGIN
    DECLARE src_count INT DEFAULT 0;

    DROP TEMPORARY TABLE IF EXISTS tmp_aries_chart_father;
    DROP TEMPORARY TABLE IF EXISTS tmp_chart_from_temp_new;
    DROP TEMPORARY TABLE IF EXISTS tmp_chart_from_temp_parent;

    SELECT COUNT(*) INTO src_count FROM tmp_aries_chart;
    IF src_count = 0 THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'No se pudo clonar el maestro de cuentas';
    END IF;

    INSERT IGNORE INTO accounts_names (`name`)
    SELECT DISTINCT t.name FROM tmp_aries_chart t;

    INSERT INTO accounts (
        account_name_id, father_account, company_id, account_type, account_guide,
        editable, detail, created_at, updated_at, updated_by, active
    )
    SELECT
        n.account_name_id,
        NULL,
        ToCompany,
        t.account_tag,
        t.account_guide,
        t.editable,
        t.memo,
        NOW(), NOW(),
        t.updated_by,
        1
    FROM tmp_aries_chart t
    INNER JOIN accounts_names n ON n.name = t.name
    ORDER BY t.seq;

    CREATE TEMPORARY TABLE tmp_chart_from_temp_new (
        seq INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
        new_id INT UNSIGNED NOT NULL
    );

    INSERT INTO tmp_chart_from_temp_new (new_id)
    SELECT a.account_id
    FROM accounts a
    WHERE a.company_id = ToCompany
    ORDER BY a.account_id;

    CREATE TEMPORARY TABLE tmp_aries_chart_father AS SELECT seq, old_id, father_old FROM tmp_aries_chart;
    CREATE TEMPORARY TABLE tmp_chart_from_temp_parent AS SELECT seq, new_id FROM tmp_chart_from_temp_new;

    UPDATE accounts dest
    INNER JOIN tmp_chart_from_temp_new n ON n.new_id = dest.account_id
    INNER JOIN tmp_aries_chart t ON t.seq = n.seq
    LEFT JOIN tmp_aries_chart_father p ON p.old_id = t.father_old
    LEFT JOIN tmp_chart_from_temp_parent np ON np.seq = p.seq
    SET dest.father_account = np.new_id
    WHERE dest.company_id = ToCompany;
END
";
    }
}
