namespace AriesContador.Data.Query
{
    public struct Query
    {
        public static partial class AdministrationQuery
        {
            public const string JuridicPerson = @"
SELECT
T0.company_id AS 'Code',
T0.type_id+0 AS 'CompanyType',
T0.type_id+0 AS 'IdType',
T0.number_id AS 'NumberId',
T0.name AS 'CompanyName',
T0.money_type+0 AS 'MoneyType',
T0.op1 AS 'Op1',
T0.op2 AS 'Op2',
T0.address AS 'Address',
T0.website AS 'WebSite',
T0.mail AS 'Mail',
T0.phone_number1 AS 'PhoneNumber1',
T0.phone_number2 AS 'PhoneNumber2',
T0.notes AS 'Notes',
T0.user_id AS 'CreatedBy',
T0.created_at AS 'CreatedAt',
T0.updated_at AS 'UpdateAt',
T0.active AS 'Active'
FROM companies AS T0 WHERE T0.type_id = 1 
";

            public const string FisicPerson = @"
SELECT
T0.company_id AS 'Code',
T0.type_id+0 AS 'CompanyType',
T0.type_id+0 AS 'IdType',
T0.number_id AS 'NumberId',
T0.name AS 'CompanyName',
T0.money_type+0 AS 'MoneyType',
T0.op1 AS 'Op1',
T0.op2 AS 'Op2',
T0.address AS 'Address',
T0.website AS 'WebSite',
T0.mail AS 'Mail',
T0.phone_number1 AS 'PhoneNumber1',
T0.phone_number2 AS 'PhoneNumber2',
T0.notes AS 'Notes',
T0.user_id AS 'CreatedBy',
T0.created_at AS 'CreatedAt',
T0.updated_at AS 'UpdateAt',
T0.active AS 'Active'
FROM companies AS T0 
WHERE T0.type_id <> 1
";

        }
    }
}
