using System;
using System.Collections.Generic;

namespace AriesContador.Core.Models.JournalEntries
{
    public class JournalEntryDto
    {
        public int Id { get; set; }
        public int Number { get; set; }
        public int PostingPeriodId { get; set; }
        public string PostingPeriodName { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string UpdatedBy { get; set; }
        public JournalEntryStatus Status { get; set; }
        public bool Active { get; set; }
        public decimal TotalDebits { get; set; }
        public decimal TotalCredits { get; set; }
        public bool IsBalanced => TotalDebits == TotalCredits;
        public List<JournalEntryLineDto> Lines { get; set; } = new List<JournalEntryLineDto>();
    }
}
