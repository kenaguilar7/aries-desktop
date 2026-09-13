namespace AriesContador.Core.Models.PointOfSale
{
    public class OpenCashRegisterRequest
    {
        public int RegisterId { get; set; }

        public decimal OpeningAmount { get; set; }

        public string Notes { get; set; }
    }

    public class CloseCashRegisterRequest
    {
        public int RegisterId { get; set; }

        public decimal DeclaredAmount { get; set; }

        public string Notes { get; set; }
    }
}
