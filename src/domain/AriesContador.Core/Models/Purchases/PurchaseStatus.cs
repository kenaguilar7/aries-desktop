namespace AriesContador.Core.Models.Purchases
{
    public enum PurchaseStatus
    {
        Draft = 0,
        Confirmed = 1
    }

    public static class PurchaseStatusNames
    {
        public const string Draft = "draft";
        public const string Confirmed = "confirmed";

        public static string ToDb(PurchaseStatus status) =>
            status == PurchaseStatus.Confirmed ? Confirmed : Draft;

        public static PurchaseStatus FromDb(string value) =>
            string.Equals(value, Confirmed, System.StringComparison.OrdinalIgnoreCase)
                ? PurchaseStatus.Confirmed
                : PurchaseStatus.Draft;

        public static string ToDisplay(PurchaseStatus status) =>
            status == PurchaseStatus.Confirmed ? "Confirmada" : "Borrador";
    }
}
