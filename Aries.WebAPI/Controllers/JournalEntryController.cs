using Aries.WebServices.FinancialServices;
using AriesContador.Core.Models.JournalEntries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Aries.WebAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class JournalEntryController : ControllerBase
    {
        private readonly IJournalEntryService _journalEntryService; 
        public JournalEntryController(IJournalEntryService journalEntryService)
        {
            _journalEntryService = journalEntryService;
        }

        [HttpPost("CreateJournalEntry")]
        public async Task<IActionResult> CreateJournalEntry([FromBody] JournalEntry journalEntry)
        {
            var id = await _journalEntryService.CreateJournalEntry(journalEntry);
            return Ok(id);
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

        [HttpPost("UpdateJournalEntry")]
        public async Task<IActionResult> UpdateJournalEntry([FromBody] JournalEntry journalEntry)
        {
            await _journalEntryService.UpdateJournalEntry(journalEntry);
            return Ok();
        }

        [HttpPost("DeleteJournalEntry")]
        public async Task<IActionResult> DeleteJournalEntry([FromBody] JournalEntry journalEntry)
        {
            await _journalEntryService.DeleteJournalEntry(journalEntry);    
            return Ok();
        }
    }
}
