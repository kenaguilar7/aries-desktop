using AriesContador.Core.Models.PointOfSale;

namespace Aries.Contabilidad.Services
{
    public interface IPosAccountingClient
    {
        Task<PosAccountMap> GetAccountMapAsync(string companyId);
        Task SaveAccountMapAsync(PosAccountMap map);
        Task<PosAccountMap> EnsureSuggestedAccountsAsync(string companyId);
        Task<List<SalesRegisterSession>> GetUnpostedSessionsAsync(string companyId);
        Task<PosPostingPreview> PreviewSessionAsync(int sessionId);
        Task<PosPostingPreview> PostSessionAsync(int sessionId);
        Task<List<PosSessionReconciliationRow>> GetReconciliationAsync(string companyId);
    }
}
