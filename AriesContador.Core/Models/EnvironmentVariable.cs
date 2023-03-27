using AriesContador.Core.Models.Users;
using System;
using System.Collections.Generic;
using System.Text;

namespace AriesContador.Core.Models
{
    public static class EnvironmentVariable
    {

        public static string ApiUrl { get; set; }
        public static WebToken ApiToken { get; set; } = new WebToken();
    }
}
