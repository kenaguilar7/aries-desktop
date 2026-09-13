using AriesContador.Core.Models.Companies;

namespace Aries.Contabilidad.Services
{
    public interface IClientCompanyService
    {
        Task<IEnumerable<Company>> GetAllCompaniesAsync();
        Task<Company> GetCompanyByCodeAsync(string code);
        Task<Company> CreateCompanyAsync(Company company);
        Task UpdateCompanyAsync(Company company);
        Task DeleteCompanyAsync(string code);
        Task<string> GetNextCompanyCode();
    }
}
