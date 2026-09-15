namespace AriesContador.Core.Models.Purchases
{
    /// <summary>
    /// Medio de liquidación de una factura de compra.
    /// Separado de <c>PaymentMethod</c> del POS para no mezclar venta y compra.
    /// </summary>
    public enum PurchaseSettlement
    {
        Cash = 1,
        Card = 2,
        Transfer = 3,
        OnAccount = 4
    }

    public static class PurchaseSettlementNames
    {
        public const string Cash = "efectivo";
        public const string Card = "tarjeta";
        public const string Transfer = "transferencia";
        public const string OnAccount = "credito";

        public static string ToDb(PurchaseSettlement method)
        {
            switch (method)
            {
                case PurchaseSettlement.Card:
                    return Card;
                case PurchaseSettlement.Transfer:
                    return Transfer;
                case PurchaseSettlement.OnAccount:
                    return OnAccount;
                default:
                    return Cash;
            }
        }

        public static PurchaseSettlement FromDb(string value)
        {
            if (string.Equals(value, Card, System.StringComparison.OrdinalIgnoreCase))
                return PurchaseSettlement.Card;
            if (string.Equals(value, Transfer, System.StringComparison.OrdinalIgnoreCase))
                return PurchaseSettlement.Transfer;
            if (string.Equals(value, OnAccount, System.StringComparison.OrdinalIgnoreCase)
                || string.Equals(value, "cuenta", System.StringComparison.OrdinalIgnoreCase))
                return PurchaseSettlement.OnAccount;
            return PurchaseSettlement.Cash;
        }

        public static string ToDisplay(PurchaseSettlement method)
        {
            switch (method)
            {
                case PurchaseSettlement.Card:
                    return "Tarjeta";
                case PurchaseSettlement.Transfer:
                    return "Transferencia";
                case PurchaseSettlement.OnAccount:
                    return "Crédito (CxP)";
                default:
                    return "Efectivo";
            }
        }
    }
}
