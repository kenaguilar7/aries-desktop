using Aries.WebServices.FinancialServices;
using Microsoft.AspNetCore.Mvc;

namespace Aries.WebAPI.Controllers
{
    //[Authorize]
    [ApiController]
    [Route("[controller]")]
    public class JournalEntryController : ControllerBase
    {
        private readonly IJournalEntryService _journalEntryService; 
        public JournalEntryController(IJournalEntryService journalEntryService)
        {
            _journalEntryService = journalEntryService;
        }

        [HttpGet("GetConsecutiveNumber/{postingPeriodId}")]
        public async Task<IActionResult> GetConsecutiveNumber(int postingPeriodId)
        {
            var newConsecutive = await _journalEntryService.CreateJournalEntryConsecutive(postingPeriodId); 
            return Ok(newConsecutive);
        }

        [HttpGet("GetJournalEntries/{postingPeriodId}")]
        public async Task<IActionResult> GetJournalEntries(int postingPeriodId)
        {
            var result = await _journalEntryService.GetJournalEntries(postingPeriodId);
            return Ok(result);
        }

    }
}
