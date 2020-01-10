using Autodesk.AutoCAD.DatabaseServices;
using JsonParse;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jCAD.PID_Builder
{
  public class Program
  {

    static void Main(string[] args)
    {
      JObject o = JObject.Parse(@"{
  'CPU': 'Intel',
  'Drives': [
    'DVD read/writer',
    '500 gigabyte hard drive'
  ]
}");

      string jsonStringText = System.IO.File.ReadAllText(@"C:\Users\jszomor\source\repos\jszomorCAD\jCAD.PID_Builder\JsonStringBuilder.json");

      JObject jsonString = JObject.Parse(jsonStringText);

      Console.WriteLine(jsonString["Blocks"]);

      // get JSON result objects into a list
      //IList<string> results = jsonString["Blocks"].Select(t => (string)t).ToList();
      //IList<JToken> results = jsonString["Blocks"][0].Children().ToList();
      // serialize JSON results into .NET objects
      //IList<JsonBlockProperty> searchResults = new List<JsonBlockProperty>();
      //foreach (string result in results)
      //{
      //  //Type type = typeof(string);
      //  //var i1 = System.Convert.ChangeType(result.ToString(), type);
      //  //var i3 = Convert.ToString(i1);

      //  //JsonDeser(result, equipmentName);
      //  // JToken.ToObject is a helper method that uses JsonSerializer internally
      //  //JsonBlockProperty searchResult = result.ToObject<JsonBlockProperty>();
      //  //searchResults.Add(searchResult);
      //}

      ////JsonDeser(jsonStringText, equipmentName);



      //foreach (var item in results)
      //{
      //  Console.WriteLine(item);
      //}
      Console.ReadKey();
    }
  }
}
