using System;
using System.Linq;
using System.Threading.Tasks;
using AriesContador.Core.Models.JournalEntries;
using AriesContador.Core.Models.PostingPeriods;
using AriesContador.Services;
using AriesContador.Tests.Fakes;
using Xunit;

namespace AriesContador.Tests.JournalTests
{
    public class FinancialServiceJournalTests
    {
        [Fact]
        public async Task CreateJournalEntry_does_not_validate_balance()
        {
            var uow = new FakeUnitOfWork();
            var svc = new FinancialService(uow);
            var unbalanced = new JournalEntry
            {
                JournalEntryLines = { new JournalEntryLine { Amount = 10 } }
            };

            await svc.CreateJournalEntryAsync(unbalanced);

            var saved = Assert.Single(uow.JournalEntries.Items);
            Assert.Equal(saved.Id, unbalanced.JournalEntryLines.Single().JournalEntryId);
            Assert.Empty(uow.JournalEntryLines.Items);
        }

        [Fact]
        public async Task CreateJournalEntry_persists_header_and_lines_together()
        {
            var uow = new FakeUnitOfWork();
            var svc = new FinancialService(uow);
            var entry = new JournalEntry
            {
                JournalEntryLines =
                {
                    new JournalEntryLine { Amount = 10, DebOrCred = AriesContador.Core.Models.Utils.DebOrCred.Debito },
                    new JournalEntryLine { Amount = 10, DebOrCred = AriesContador.Core.Models.Utils.DebOrCred.Credito }
                }
            };

            await svc.CreateJournalEntryAsync(entry);

            var saved = Assert.Single(uow.JournalEntries.Items);
            Assert.Equal(2, saved.JournalEntryLines.Count);
            Assert.All(saved.JournalEntryLines, line =>
            {
                Assert.Equal(saved.Id, line.JournalEntryId);
                Assert.NotEqual(0, line.Id);
            });
            Assert.Empty(uow.JournalEntryLines.Items);
        }

        [Fact]
        public async Task CreateJournalEntry_does_not_keep_header_when_line_insert_fails()
        {
            var uow = new FakeUnitOfWork();
            uow.JournalEntries.FailAfterHeader = true;
            var svc = new FinancialService(uow);
            var entry = new JournalEntry
            {
                JournalEntryLines =
                {
                    new JournalEntryLine { Amount = 10 },
                    new JournalEntryLine { Amount = 10 }
                }
            };

            await Assert.ThrowsAsync<InvalidOperationException>(() => svc.CreateJournalEntryAsync(entry));
            Assert.Empty(uow.JournalEntries.Items);
        }

        [Fact]
        public async Task CreateJournalEntryConsecutive_returns_repository_number()
        {
            var uow = new FakeUnitOfWork();
            uow.JournalEntries.ConsecutiveNumber = 42;
            var svc = new FinancialService(uow);

            Assert.Equal(42, await svc.CreateJournalEntryConsecutiveAsync(9));
        }

        [Fact]
        public async Task RestoreJournalEntry_forwards_payload_to_repository()
        {
            var uow = new FakeUnitOfWork();
            var svc = new FinancialService(uow);
            var entry = new JournalEntry { Id = 11, UpdatedBy = 3 };

            await svc.RestoreJournalEntryAsync(entry);

            Assert.Same(entry, uow.JournalEntries.Restored.Single());
        }

        [Fact]
        public async Task RestoreJournalEntryLine_forwards_payload_to_repository()
        {
            var uow = new FakeUnitOfWork();
            var svc = new FinancialService(uow);
            var line = new JournalEntryLine { Id = 8, UpdatedBy = 3 };

            await svc.RestoreJournalEntryLineAsync(line);

            Assert.Same(line, uow.JournalEntryLines.Restored.Single());
        }

        [Fact]
        public async Task DeleteJournalEntry_soft_deletes_via_remove()
        {
            var uow = new FakeUnitOfWork();
            var svc = new FinancialService(uow);
            var entry = new JournalEntry { Id = 5 };

            await svc.DeleteJournalEntryAsync(entry);

            Assert.Same(entry, uow.JournalEntries.Removed.Single());
        }

        [Fact]
        public async Task UpdatedJournalEntryPeriod_assigns_new_consecutive_then_updates()
        {
            var uow = new FakeUnitOfWork();
            uow.JournalEntries.ConsecutiveNumber = 17;
            var svc = new FinancialService(uow);
            var entry = new JournalEntry { Id = 4, Number = 2, PostingPeriodId = 99 };

            await svc.UpdatedJournalEntryPeriodAsync(entry);

            Assert.Equal(17, entry.Number);
            Assert.Same(entry, uow.JournalEntries.Updated.Single());
        }

        [Fact]
        public async Task ClosePostingPeriod_forwards_to_repository()
        {
            var uow = new FakeUnitOfWork();
            var svc = new FinancialService(uow);
            var closing = new PostingPeriodEndClosing { CompanyId = "C001", Amount = 1500m };

            await svc.ClosePostingPeriodAsync(closing);

            Assert.Same(closing, uow.PostingPeriods.Closed.Single());
        }

        [Fact]
        public async Task CreatePostingPeriod_rejects_duplicate_year_month()
        {
            var uow = new FakeUnitOfWork();
            uow.PostingPeriods.Items.Add(new PostingPeriod
            {
                CompanyId = "C001",
                Date = new DateTime(2024, 3, 15)
            });
            var svc = new FinancialService(uow);

            var duplicate = new PostingPeriod
            {
                CompanyId = "C001",
                Date = new DateTime(2024, 3, 1)
            };

            var ex = await Assert.ThrowsAsync<Exception>(() => svc.CreatePostingPeriodAsync(duplicate));
            Assert.Equal("Periodo contable con fechas repetidas", ex.Message);
            Assert.Single(uow.PostingPeriods.Items);
        }

        [Fact]
        public async Task CreatePostingPeriod_allows_different_month()
        {
            var uow = new FakeUnitOfWork();
            uow.PostingPeriods.Items.Add(new PostingPeriod
            {
                CompanyId = "C001",
                Date = new DateTime(2024, 3, 1)
            });
            var svc = new FinancialService(uow);
            var next = new PostingPeriod { CompanyId = "C001", Date = new DateTime(2024, 4, 1) };

            await svc.CreatePostingPeriodAsync(next);

            Assert.Equal(2, uow.PostingPeriods.Items.Count);
        }
    }
}
