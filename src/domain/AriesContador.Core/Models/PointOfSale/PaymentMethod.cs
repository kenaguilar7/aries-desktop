namespace AriesContador.Core.Models.PointOfSale
{
    public enum PaymentMethod
    {
        Efectivo = 1,
        Tarjeta = 2,
        Transferencia = 3
    }

    public static class PaymentMethodNames
    {
        public const string Efectivo = "efectivo";
        public const string Tarjeta = "tarjeta";
        public const string Transferencia = "transferencia";

        public static string ToDb(PaymentMethod method)
        {
            switch (method)
            {
                case PaymentMethod.Tarjeta:
                    return Tarjeta;
                case PaymentMethod.Transferencia:
                    return Transferencia;
                default:
                    return Efectivo;
            }
        }

        public static PaymentMethod FromDb(string value)
        {
            if (string.Equals(value, Tarjeta, System.StringComparison.OrdinalIgnoreCase))
                return PaymentMethod.Tarjeta;
            if (string.Equals(value, Transferencia, System.StringComparison.OrdinalIgnoreCase))
                return PaymentMethod.Transferencia;
            return PaymentMethod.Efectivo;
        }
    }
}
