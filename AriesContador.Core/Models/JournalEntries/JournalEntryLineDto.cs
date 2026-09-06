using System;
using AriesContador.Core.Models.Utils;

namespace AriesContador.Core.Models.JournalEntries
{
    public class JournalEntryLineDto
    {
        public int Id { get; set; }
        public int AccountId { get; set; }
        public string AccountName { get; set; }
        public string AccountPath { get; set; }
        public int JournalEntryId { get; set; }
        public string Reference { get; set; }
        public string Memo { get; set; }
        public DateTime Date { get; set; }
        public Currency Currency { get; set; }
        public decimal RateAmount { get; set; }
        public decimal Amount { get; set; }
        public decimal ForeignAmount { get; set; }
        public DebOrCred DebOrCred { get; set; }
    }
}
