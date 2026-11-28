namespace MicroservicesWithKafka.Models
{
    /// <summary>
    /// Whose challans a role dashboard may show: one branch (BR) or one ASC (SF/SSD).
    /// Always built from the login token; exactly one of the two values is set.
    /// </summary>
    public sealed class DashboardScope
    {
        public int? BranchId { get; }
        public int? SfId { get; }

        private DashboardScope(int? branchId, int? sfId)
        {
            BranchId = branchId;
            SfId = sfId;
        }

        public static DashboardScope ForBranch(int branchId) => new(branchId, null);

        public static DashboardScope ForAsc(int sfId) => new(null, sfId);
    }
}
