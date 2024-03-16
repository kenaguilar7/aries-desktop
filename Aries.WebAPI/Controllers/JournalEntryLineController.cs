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

        [HttpPost("DeleteJournalEntryLine")]
        public async Task<IActionResult> DeleteJournalEntryLine([FromBody] JournalEntryLine journalEntryLine)
        {
            await _journalEntryLineService.DeleteJournalEntryLine(journalEntryLine);    
            return Ok();
        }

        [HttpGet("FindJournalEntryLine/{journalEntryId}")]
        public async Task<IActionResult> FindJournalEntryLine(int journalEntryId)
        {
            var result = await _journalEntryLineService.GetJournalEntryLineByJournalEntryId(journalEntryId);
            return Ok(result);  
        }

        [HttpPost("UpdateJournalEntryLine")]
        public async Task<IActionResult> UpdateJournalEntryLine([FromBody] JournalEntryLine journalEntryLine)
        {
            await _journalEntryLineService.UpdateJournalEntryLine(journalEntryLine);
            return Ok(); 
        }
    }
}
