using System;
using AriesContador.Core.Models.Utils;
using System.Collections;
using System.Collections.Generic;
using AriesContador.Core.Models.PostingPeriods;
using AriesContador.Core.Models.Accounts;

namespace AriesContador.Core.Models.Companies
{
    public class Company : BaseModel
    {
        public string Code { get; set; }

        public string Name { get; set; }

        public CompanyType CompanyType { get; set;  }

        public IdType IdType { get; set; }

        public string IdNumber { get; set; }

        public string Op1 { get; set; }

        public string Op2 { get; set; }

        public string Address { get; set; }

        public string Mail { get; set; }

        public string PhoneNumber1 { get; set; }

        public string PhoneNumber2 { get; set; }

        public string Memo { get; set; }

        public string Web { get; set; }

        public CurrencyTypeCompany CurrencyType { get; set; }

        public IEnumerable<PostingPeriod> PostingPeriods { get; set; } = new List<PostingPeriod>();

        public IEnumerable<Account> Account { get; set; } = new List<Account>(); 

        public override string ToString()
        {
            return $"{ Name.ToUpper()}-{Code}";
        }
    }
}
