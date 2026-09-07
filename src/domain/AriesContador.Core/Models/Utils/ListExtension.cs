using AriesContador.Core.Models.PostingPeriods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace AriesContador.Core.Models.Utils
{
    public static class ListExtension
    {
        public static T DeepClone<T>(this T theObject)
        {
            string jsonData = JsonConvert.SerializeObject(theObject);
            return JsonConvert.DeserializeObject<T>(jsonData);
        }
    }
}
