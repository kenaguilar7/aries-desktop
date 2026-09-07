using System.Collections.Generic;

namespace AriesContador.Data.Migrations
{
    public sealed class MigrationResult
    {
        public MigrationResult(IReadOnlyList<string> appliedIds, IReadOnlyList<string> alreadyAppliedIds)
        {
            AppliedIds = appliedIds;
            AlreadyAppliedIds = alreadyAppliedIds;
        }

        public IReadOnlyList<string> AppliedIds { get; }

        public IReadOnlyList<string> AlreadyAppliedIds { get; }

        public bool HadPending => AppliedIds.Count > 0;
    }
}
