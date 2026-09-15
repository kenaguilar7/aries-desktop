using AriesContador.Core.Models.Utils;

namespace AriesContador.Core.Models.Purchases
{
    public class Supplier : BaseModel
    {
        public string CompanyId { get; set; }

        public string Name { get; set; }

        public IdType IdType { get; set; } = IdType.CEDULA_JURIDICA;

        public string NumberId { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        public string Address { get; set; }

        public string Notes { get; set; }
    }
}
