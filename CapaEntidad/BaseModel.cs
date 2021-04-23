using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaEntidad
{
    public class BaseModel
    {
        public int Id { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdateAt { get; set; }

        public int CreatedBy { get; set; }

        public int UpdatedBy { get; set; }

        public Boolean Active { get; set; }
        public int ActiveMySQL
        {
            get { return Convert.ToInt32(this.Active); }
            set { this.Active = Convert.ToBoolean(value); }
        }
    }
}
