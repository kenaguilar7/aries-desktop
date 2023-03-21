using AriesContador.Core.Models.Companies;
using CapaEntidad.Entidades.Compañias;
using CapaEntidad.Entidades.Usuarios;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaEntidad.Interfaces
{
    public interface IDao <T>
    {
        String Insert(T t,Usuario user);
        String Update(T t, Usuario user);
        String Delete(T t, Usuario user);
        List<T> GetAll(Company t,Usuario user);
        DataTable GetDataTable(Company t, Usuario user);

    }
}
