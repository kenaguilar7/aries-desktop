using AriesContador.Core.Models.Companies;
using AriesContador.Core.Models.Accounts;
using System;
using System.Collections.Generic;
using System.Text;
using AriesContador.Core.Models.PostingPeriods;

namespace AriesContador.Core.Models.Users
{
    public class User : BaseModel
    {
        public string UserName { get; set; }

        public UserType UserType { get; set; }
        
        public string IdNumber { get; set; }

        public string Name { get; set; }

        public string LastName { get; set; }

        public string MiddleName { get; set; }

        public string PhoneNumber { get; set; }

        public string Mail { get; set; }
        
        public string Memo { get; set; }

        public string Password { get; set; }

        public string CurrentToken { get; set;  }
        
        public override string ToString()
        {
            return $"{Name} {LastName} {LastName}";
        }
    }

    public class Login
    {
        public string UserId { get; set; }
        public string Password { get; set; }
    }

    public class WebToken 
    {
        public string Token { get; set; } = string.Empty; 
        public User User { get; set; }
    }

    //public class UserLogin
    //{
    //    public string Username { get; internal set; }
    //    public string Password { get; internal set; }
    //}
}
