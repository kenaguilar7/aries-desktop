using AriesContador.Core.Models.JournalEntries;
using AriesContador.Core.Repositories;
using AriesContador.Data.Internal.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AriesContador.Data.Repositories
{
    public class JournalEntryRepository : IJournalEntryRepository
    {
        private readonly IConnectionString _connectionString;
        public JournalEntryRepository(IConnectionString connectionString)
        {
            this._connectionString = connectionString;
        }

        public void Add(JournalEntry entity)
        {
            MySqlDataAccess dataAccess = new MySqlDataAccess(_connectionString);
            entity.Id = dataAccess.SaveData<JournalEntry, int>("SP_InsertJournalEntry", entity);
        }

        public async Task AddAsync(JournalEntry entity)
        {
            MySqlDataAccessAsync dataAccess = new MySqlDataAccessAsync(_connectionString);
            entity.Id = await dataAccess.SaveData<JournalEntry, int>("SP_InsertJournalEntry", entity);
        }

        public IEnumerable<JournalEntry> FindByPostingPeriodId(int pstPeriodId)
        {

            using (MySqlDataAccess dataAccess = new MySqlDataAccess(_connectionString))
            {
                dataAccess.StartTransaction();
                var output = dataAccess.LoadDataInTransaction<JournalEntry, dynamic>
                                ("SP_GetJournalEntryByPostingPeriodId", new { PostingPeriodId = pstPeriodId });

                foreach (var jEntry in output)
                {
                    var jELines = dataAccess.LoadDataInTransaction<JournalEntryLine, dynamic>
                        ("SP_GetJournalEntryLineByJournalEntryId", new { JournalEntryId = jEntry.Id });
                    jEntry.JournalEntryLines = jELines;
                }

                return output;
            }
        }

        public async Task<IEnumerable<JournalEntry>> FindByPostingPeriodIdAsync(int pstPeriodId)
        {

                MySqlDataAccessAsync dataAccess = new MySqlDataAccessAsync(_connectionString); 
            
                dataAccess.StartTransaction();
                var output = await dataAccess.LoadDataInTransaction<JournalEntry, dynamic>
                                ("SP_GetJournalEntryByPostingPeriodId", new { PostingPeriodId = pstPeriodId });
                return output;
        }

        public JournalEntry GetById(int id)
        {
            MySqlDataAccess dataAccess = new MySqlDataAccess(_connectionString);
            var output = dataAccess.LoadData<JournalEntry, dynamic>("SP_GetJournalEntryById", new { Id = id });
            return output.FirstOrDefault();
        }

        public int GetConsecutiveNumber(int postingPeriodId)
        {
            MySqlDataAccess dataAccess = new MySqlDataAccess(_connectionString);
            var newNumber = dataAccess.LoadData<int, dynamic>("SP_GetJournalEntryConsecutive", new { postingPeriodId });

            return newNumber.First();
        }

        public async Task<int> GetConsecutiveNumberAsync(int postingPeriodId)
        {
            MySqlDataAccessAsync dataAccess = new MySqlDataAccessAsync(_connectionString);
            var newNumber = await dataAccess.LoadData<int, dynamic>("SP_GetJournalEntryConsecutive", new { postingPeriodId });
            return newNumber.First();
        }

        public IEnumerable<JournalEntryDeletedReport> GetDeletedItemByDateRange(BasicReportParam reportParam)
        {
            MySqlDataAccess dataAccess = new MySqlDataAccess(_connectionString);
            return dataAccess.LoadData<JournalEntryDeletedReport, BasicReportParam>("SP_GetJournalEntryDeletedBydDateRange", reportParam); 
        }

        public void RestoreJournalEntry(JournalEntry entryLine)
        {
            MySqlDataAccess dataAccess = new MySqlDataAccess(_connectionString);
            dataAccess.SaveData<JournalEntry>("SP_RestoreJournalEntry", entryLine);
        }

        public async Task Remove(JournalEntry entity)
        {
            entity.JournalEntryLines = new List<JournalEntryLine>();
            MySqlDataAccessAsync dataAccess = new MySqlDataAccessAsync(_connectionString);
            await dataAccess.SaveData<JournalEntry>("SP_DesactivateJournalEntry", entity);
        }

        public void Update(JournalEntry entity)
        {
            var repoEntities = entity.JournalEntryLines;
            entity.JournalEntryLines = new List<JournalEntryLine>();
            MySqlDataAccess dataAccess = new MySqlDataAccess(_connectionString);
            dataAccess.SaveData<JournalEntry>("SP_UpdateJournalEntry", entity);
            entity.JournalEntryLines = repoEntities;
        }

        public async Task UpdateAsync(JournalEntry entity)
        {
            var repoEntities = entity.JournalEntryLines;
            entity.JournalEntryLines = new List<JournalEntryLine>();
            MySqlDataAccessAsync dataAccess = new MySqlDataAccessAsync(_connectionString);
            await dataAccess.SaveData<JournalEntry>("SP_UpdateJournalEntry", entity);
            entity.JournalEntryLines = repoEntities;
        }

        public async Task<int> AddAsyncReturningId(JournalEntry journalEntry)
        {
            MySqlDataAccessAsync dataAccess = new MySqlDataAccessAsync(_connectionString);
            journalEntry.Id = await dataAccess.SaveData<JournalEntry, int>("SP_InsertJournalEntry", journalEntry);
            return journalEntry.Id; 
        }
    }
}
