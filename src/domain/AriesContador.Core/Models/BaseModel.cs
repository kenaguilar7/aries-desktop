using System;

namespace AriesContador.Core.Models
{
    public abstract class BaseModel
    {
        public int Id { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdateAt { get; set; }

        public int CreatedBy { get; set; }

        public int UpdatedBy { get; set; }

        public bool Active { get; set; }
        public int ActiveMySQL
        {
            get { return Convert.ToInt32(this.Active); }
            set { this.Active = Convert.ToBoolean(value); }
        }
    }
}
