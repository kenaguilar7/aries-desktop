-- Duplicate accounts. 

INSERT INTO `aries`.`transactions_accounting`
(
`account_id`,
`accounting_entry_id`,
`reference`,
`detail`,
`balance`,
`foreign_amount`,
`balance_type`,
`money_type`,
`money_chance`,
`bill_date`,
`updated_by`)

SELECT distinct
    T0.account_id, 3219 AS 'accounting_entry_id', T0.reference, T0.detail, T0.balance, T0.foreign_amount, T0.balance_type, T0.money_type, T0.money_chance, T0.bill_date, T0.updated_by
FROM
    transactions_accounting T0
        INNER JOIN
    accounting_entries T1 USING (accounting_entry_id)
        INNER JOIN
    accounting_months T2 USING (accounting_months_id)
WHERE
    T2.company_id = 'C160'
        AND T2.month_report = '2020-03-09'
        AND T1.active = 1
        AND T1.entry_id = 7



-- Fix account's name typos
UPDATE `aries`.`accounts_names` SET `name` = 'SUPERÁVIT' WHERE (`account_name_id` = '14');
UPDATE `aries`.`accounts_names` SET `name` = 'UTILIDADES ACUMULADAS' WHERE (`account_name_id` = '15');
UPDATE `aries`.`accounts_names` SET `name` = 'ACTIVOS INTANGIBLES' WHERE (`account_name_id` = '25');
UPDATE `aries`.`accounts_names` SET `name` = 'PAPELERIA Y UTILES DE OFICINA' WHERE (`account_name_id` = '36');
UPDATE `aries`.`accounts_names` SET `name` = 'SERVICIOS PÚBLICOS' WHERE (`account_name_id` = '38');
UPDATE `aries`.`accounts_names` SET `name` = 'DEPRECIACIONES' WHERE (`account_name_id` = '39');
UPDATE `aries`.`accounts_names` SET `name` = 'TRANSPORTES' WHERE (`account_name_id` = '43');
UPDATE `aries`.`accounts_names` SET `name` = 'ASEO E HIGIENE' WHERE (`account_name_id` = '44');
UPDATE `aries`.`accounts_names` SET `name` = 'IMPUESTO SOCIEDADES-REGISTRO PÚBLICO' WHERE (`account_name_id` = '45');
UPDATE `aries`.`accounts_names` SET `name` = 'VIÁTICOS Y COMISIONES' WHERE (`account_name_id` = '50');
UPDATE `aries`.`accounts_names` SET `name` = 'COMISIONES TARJETAS DE CRÉDITO' WHERE (`account_name_id` = '52');
UPDATE `aries`.`accounts_names` SET `name` = 'COMISIÓN DE SERVICIO' WHERE (`account_name_id` = '53');

select *
from accounts t0
    inner join accounts_names t1 using(account_name_id)
where t1.name in (
        'SUPERÁVIT_2',
        'UTILIDADES ACUMULADAS_2',
        'ACTIVOS INTANGIBLES_2',
        'PAPELERIA Y UTILES DE OFICINA_2',
        'SERVICIOS PÚBLICOS_2',
        'DEPRECIACIONES_2',
        'TRANSPORTES_2',
        'ASEO E HIGIENE_2',
        'IMPUESTO SOCIEDADES-REGISTRO PÚBLICO_2',
        'VIÁTICOS Y COMISIONES_2',
        'COMISIONES TARJETAS DE CRÉDITO_2',
        'COMISIÓN DE SERVICIO_2'
    )