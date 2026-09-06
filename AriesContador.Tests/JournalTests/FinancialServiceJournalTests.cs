using System;
using System.Linq;
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
        public void CreateJournalEntry_does_not_validate_balance()
        {
            var uow = new FakeUnitOfWork();
            var svc = new FinancialService(uow);
            var unbalanced = new JournalEntry
            {
                JournalEntryLines = { new JournalEntryLine { Amount = 10 } }
            };

            svc.CreateJournalEntry(unbalanced);

            var saved = Assert.Single(uow.JournalEntries.Items);
            Assert.Equal(saved.Id, unbalanced.JournalEntryLines.Single().JournalEntryId);
            Assert.Empty(uow.JournalEntryLines.Items);
        }

        [Fact]
        public void CreateJournalEntry_persists_header_and_lines_together()
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

            svc.CreateJournalEntry(entry);

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
        public void CreateJournalEntry_does_not_keep_header_when_line_insert_fails()
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

            Assert.Throws<InvalidOperationException>(() => svc.CreateJournalEntry(entry));
            Assert.Empty(uow.JournalEntries.Items);
        }

        [Fact]
        public void CreateJournalEntryConsecutive_returns_repository_number()
        {
            var uow = new FakeUnitOfWork();
            uow.JournalEntries.ConsecutiveNumber = 42;
            var svc = new FinancialService(uow);

            Assert.Equal(42, svc.CreateJournalEntryConsecutive(9));
        }

        [Fact]
        public void RestoreJournalEntry_forwards_payload_to_repository()
        {
            var uow = new FakeUnitOfWork();
            var svc = new FinancialService(uow);
            var entry = new JournalEntry { Id = 11, UpdatedBy = 3 };

            svc.RestoreJournalEntry(entry);

            Assert.Same(entry, uow.JournalEntries.Restored.Single());
        }

        [Fact]
        public void RestoreJournalEntryLine_forwards_payload_to_repository()
        {
            var uow = new FakeUnitOfWork();
            var svc = new FinancialService(uow);
            var line = new JournalEntryLine { Id = 8, UpdatedBy = 3 };

            svc.RestoreJournalEntryLine(line);

            Assert.Same(line, uow.JournalEntryLines.Restored.Single());
        }

        [Fact]
        public void DeleteJournalEntry_soft_deletes_via_remove()
        {
            var uow = new FakeUnitOfWork();
            var svc = new FinancialService(uow);
            var entry = new JournalEntry { Id = 5 };

            svc.DeleteJournalEntry(entry);

            Assert.Same(entry, uow.JournalEntries.Removed.Single());
        }

        [Fact]
        public void UpdatedJournalEntryPeriod_assigns_new_consecutive_then_updates()
        {
            var uow = new FakeUnitOfWork();
            uow.JournalEntries.ConsecutiveNumber = 17;
            var svc = new FinancialService(uow);
            var entry = new JournalEntry { Id = 4, Number = 2, PostingPeriodId = 99 };

            svc.UpdatedJournalEntryPeriod(entry);

            Assert.Equal(17, entry.Number);
            Assert.Same(entry, uow.JournalEntries.Updated.Single());
        }

        [Fact]
        public void ClosePostingPeriod_forwards_to_repository()
        {
            var uow = new FakeUnitOfWork();
            var svc = new FinancialService(uow);
            var closing = new PostingPeriodEndClosing { CompanyId = "C001", Amount = 1500m };

            svc.ClosePostingPeriod(closing);

            Assert.Same(closing, uow.PostingPeriods.Closed.Single());
        }

        [Fact]
        public void CreatePostingPeriod_rejects_duplicate_year_month()
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

            var ex = Assert.Throws<Exception>(() => svc.CreatePostingPeriod(duplicate));
            Assert.Equal("Periodo contable con fechas repetidas", ex.Message);
            Assert.Single(uow.PostingPeriods.Items);
        }

        [Fact]
        public void CreatePostingPeriod_allows_different_month()
        {
            var uow = new FakeUnitOfWork();
            uow.PostingPeriods.Items.Add(new PostingPeriod
            {
                CompanyId = "C001",
                Date = new DateTime(2024, 3, 1)
            });
            var svc = new FinancialService(uow);
            var next = new PostingPeriod { CompanyId = "C001", Date = new DateTime(2024, 4, 1) };

            svc.CreatePostingPeriod(next);

            Assert.Equal(2, uow.PostingPeriods.Items.Count);
        }
    }
}
