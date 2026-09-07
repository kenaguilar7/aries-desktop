using AriesContador.Core.Models.JournalEntries;
using AriesContador.Core.Models.Utils;
using Xunit;

namespace AriesContador.Tests.JournalTests
{
    public class JournalEntryBalanceTests
    {
        [Fact]
        public void Cuadrado_when_debit_equals_credit_amounts()
        {
            var entry = new JournalEntry
            {
                JournalEntryLines =
                {
                    Line(DebOrCred.Debito, 150.50m),
                    Line(DebOrCred.Credito, 100m),
                    Line(DebOrCred.Credito, 50.50m)
                }
            };

            Assert.True(entry.Cuadrado);
            Assert.Equal(150.50m, entry.DebitosColones);
            Assert.Equal(150.50m, entry.CreditosColones);
        }

        [Fact]
        public void Not_cuadrado_when_debit_differs_from_credit()
        {
            var entry = new JournalEntry
            {
                JournalEntryLines =
                {
                    Line(DebOrCred.Debito, 100m),
                    Line(DebOrCred.Credito, 99.99m)
                }
            };

            Assert.False(entry.Cuadrado);
        }

        [Fact]
        public void Empty_entry_is_cuadrado_zero_equals_zero()
        {
            Assert.True(new JournalEntry().Cuadrado);
        }

        [Fact]
        public void CanNavigateAway_allows_unsaved_draft()
        {
            var draft = new JournalEntry { Id = 0 };
            draft.JournalEntryLines.Add(Line(DebOrCred.Debito, 10m));

            Assert.False(draft.Cuadrado);
            Assert.True(draft.CanNavigateAway());
        }

        [Fact]
        public void CanNavigateAway_blocks_persisted_unbalanced_entry()
        {
            var entry = new JournalEntry { Id = 7 };
            entry.JournalEntryLines.Add(Line(DebOrCred.Debito, 10m));

            Assert.False(entry.CanNavigateAway());
        }

        [Fact]
        public void ApplyStatusFromBalance_approved_when_cuadrado()
        {
            var entry = new JournalEntry
            {
                JournalEntryLines =
                {
                    Line(DebOrCred.Debito, 20m),
                    Line(DebOrCred.Credito, 20m)
                }
            };

            entry.ApplyStatusFromBalance();
            Assert.Equal(JournalEntryStatus.Approved, entry.JournalEntryStatus);
        }

        [Fact]
        public void ApplyStatusFromBalance_progress_when_not_cuadrado()
        {
            var entry = new JournalEntry
            {
                JournalEntryLines = { Line(DebOrCred.Debito, 20m) }
            };

            entry.ApplyStatusFromBalance();
            Assert.Equal(JournalEntryStatus.Progress, entry.JournalEntryStatus);
        }

        private static JournalEntryLine Line(DebOrCred side, decimal amount) =>
            new JournalEntryLine { DebOrCred = side, Amount = amount };
    }
}
