namespace Aries.Contabilidad.Models
{
    public class JournalEntryFilter
    {
        public string SearchTerm { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? StartNumber { get; set; }
        public int? EndNumber { get; set; }
    }
}
