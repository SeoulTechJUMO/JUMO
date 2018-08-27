using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace ChordMagicianTest
{
    public class Class1
    {
        static void jsonParser()
        {
            var json = new JObject();
            json.Add("id", "Luna");
            json.Add("name", "Silver");
            json.Add("age", 19);
            Console.WriteLine(json.ToString());
        }
    }
}
