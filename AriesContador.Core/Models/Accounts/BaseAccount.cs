using AriesContador.Core.Models.Accounts;
using System;
using System.Collections.Generic;
using System.Text;

namespace AriesContador.Core.Models.Accounts
{
    public abstract class BaseAccount : BaseModel
    {
        public string Name { get; set; }

        public string Memo { get; set; }

        public bool Editable { get; set; }

        public int EditableMySql
        {
            get { return Convert.ToInt32(this.Editable); }
            set { this.Editable = Convert.ToBoolean(value); }
        }

        public AccountTag AccountTag { get; set; }

        public AccountType AccountType { get; set; }

        public string CompanyId { get; set; }

        public string PathDirection { get; set; }
    }
}
