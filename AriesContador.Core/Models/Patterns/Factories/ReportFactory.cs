using AriesContador.Core.Models.Patterns.Command;
using System;
using System.Collections.Generic;
using System.Text;

namespace AriesContador.Core.Models.Patterns.Factories
{
    public interface IReportFactory
    {
        IActionClass Create(); 
    }

    public class ReportEstadoResultadoIntgral: IReportFactory
    {
        public ReportEstadoResultadoIntgral()
        {

        }

        public IActionClass Create()
        {
            throw new NotImplementedException();
        }
    }


}
