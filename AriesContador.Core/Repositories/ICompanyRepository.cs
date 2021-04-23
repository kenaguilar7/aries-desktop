using AriesContador.Core.Models.Companies;
using System;
using System.Collections.Generic;
using System.Text;

namespace AriesContador.Core.Repositories
{
    public interface ICompanyRepository : IRepository<Company> 
    {
        IEnumerable<Company> GetAll(); 
       string GetConsecutive(); 
    }
}
