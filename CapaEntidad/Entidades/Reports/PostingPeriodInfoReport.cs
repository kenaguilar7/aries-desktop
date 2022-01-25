using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using ClosedXML.Attributes;

namespace CapaEntidad.Entidades.Reports
{
    public class PostingPeriodInfoReport
    {
        [DisplayName("Periodo Contable")]
        public string AccountPeriodName { get; set; }

        [DisplayName("Estado")]
        public string Status { get; set; }

        [DisplayName("Fecha Creación")]
        public string CreatedDate { get; set; }

        [DisplayName("Cerrado")]
        public string ClosedDate { get; set; }

        [DisplayName("Usuario")]
        public string UserName { get; set; }


    }
}
