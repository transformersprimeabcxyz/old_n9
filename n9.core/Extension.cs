using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace n9.core
{
    public static class Extension
    {
        public static void Dump(this object obj)
        {
            var settings =new JsonSerializerSettings();
            settings.TypeNameHandling = TypeNameHandling.Objects;

            Console.WriteLine(obj.GetType().Name + " " + 
                JsonConvert.SerializeObject(obj, Formatting.Indented, settings));
        }
    }
}
