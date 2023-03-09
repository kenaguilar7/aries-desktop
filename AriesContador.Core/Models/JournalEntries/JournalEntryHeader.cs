using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AriesContador.Core.Models.JournalEntries
{
    public abstract class JournalEntryHeader : BaseModel
    {
        public int Number { get; set; }

        public JournalEntryStatus JournalEntryStatus { get; set; }

        public int PostingPeriodId { get; set; }
    }
}
