using System;
using System.Collections.Generic;
using System.Text;

namespace Aries.Core.Services
{
    public interface IJournalEntryService
    {
        string TestMethod();
    }
    public class JournalEntryService: IJournalEntryService
    {
        public string TestMethod()
        {
            return "Hello word";
        }
    }
}
