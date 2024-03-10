using Aries.WebServices.FinancialServices;
using AriesContador.Core.Models.JournalEntries;
using Microsoft.AspNetCore.Mvc;

namespace Aries.WebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class JournalEntryLineController : ControllerBase
    {
        private IJournalEntryLineService _journalEntryLineService; 

        public JournalEntryLineController(IJournalEntryLineService journalEntryLineService) 
        {
            _journalEntryLineService = journalEntryLineService;
        }


        [HttpPost("CreateJournalEntryLine")]
        public async Task<IActionResult> CreateJournalEntryLine([FromBody] JournalEntryLine journalEntryLine)
        {
            var id = await _journalEntryLineService.CreateJournalEntryLine(journalEntryLine);
            return Ok(id); 
        }
    }
}
