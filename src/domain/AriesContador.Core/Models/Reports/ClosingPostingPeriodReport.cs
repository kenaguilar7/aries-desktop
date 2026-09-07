using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AriesContador.Core.Models.Reports
{
    public class ClosingPostingPeriodReport
    {
        [DisplayName("Período Desde")]
        public string FromPeriod { get; set; }

        [DisplayName("Período Hasta")]
        public string ToPeriod { get; set; }

        [DisplayName("Monto")]
        public decimal Amount { get; set; }

        [DisplayName("Notas de usuario")]
        public string UserNotes { get; set; }

        [DisplayName("Fecha movientos")]

        public DateTime CreatedAt { get; set; }

        [DisplayName("Usuario")]
        public string CreatedBy { get; set; }
    }
}
