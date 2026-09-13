namespace AriesContador.Core.Models.PointOfSale
{
    public class CashRegisterStatus
    {
        public bool Abierta { get; set; }

        public SalesRegisterSession Session { get; set; }

        public static CashRegisterStatus Closed()
        {
            return new CashRegisterStatus { Abierta = false };
        }

        public static CashRegisterStatus Open(SalesRegisterSession session)
        {
            return new CashRegisterStatus { Abierta = true, Session = session };
        }
    }
}
