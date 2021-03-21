using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaEntidad.Entidades.Ventanas
{
    public class CRUD
    {
        public bool HasInsert { get; set; }
        public bool HasUpdate { get; set; }
        public bool HasDeleted { get; set; }
        public bool HasList { get; set; }

        public CRUD(bool hasInsert, bool hasUpdate, bool hasDeleted, bool hasList)
        {
            HasInsert = hasInsert;
            HasUpdate = hasUpdate;
            HasDeleted = hasDeleted;
            HasList = hasList;
        }

        public CRUD()
        {
        }
    }
}
