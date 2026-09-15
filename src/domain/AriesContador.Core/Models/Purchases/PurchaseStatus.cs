namespace AriesContador.Core.Models.Purchases
{
    public enum PurchaseStatus
    {
        Draft = 0,
        Confirmed = 1,
        Cancelled = 2
    }

    public static class PurchaseStatusNames
    {
        public const string Draft = "draft";
        public const string Confirmed = "confirmed";
        public const string Cancelled = "cancelled";

        public static string ToDb(PurchaseStatus status)
        {
            switch (status)
            {
                case PurchaseStatus.Confirmed:
                    return Confirmed;
                case PurchaseStatus.Cancelled:
                    return Cancelled;
                default:
                    return Draft;
            }
        }

        public static PurchaseStatus FromDb(string value)
        {
            if (string.Equals(value, Confirmed, System.StringComparison.OrdinalIgnoreCase))
                return PurchaseStatus.Confirmed;
            if (string.Equals(value, Cancelled, System.StringComparison.OrdinalIgnoreCase))
                return PurchaseStatus.Cancelled;
            return PurchaseStatus.Draft;
        }

        public static string ToDisplay(PurchaseStatus status)
        {
            switch (status)
            {
                case PurchaseStatus.Confirmed:
                    return "Confirmada";
                case PurchaseStatus.Cancelled:
                    return "Anulada";
                default:
                    return "Borrador";
            }
        }
    }
}
