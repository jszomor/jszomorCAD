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
      //string jsonString = System.IO.File.ReadAllText(@"C:\Users\jszomor\source\repos\jszomorCAD\jCAD.PID_Builder\JsonStringBuilder.json");
      //Misc misc = JsonConvert.DeserializeObject<Misc>(jsonString);

      //var blockDeserialize = new BlockDeserialize();

      BlockDeserialize blockDeserialize = new BlockDeserialize();
      var eqType = blockDeserialize.BlockSearch("Position X");

      System.Diagnostics.Debug.Print($"BlockName: {eqType}");
      Console.WriteLine(eqType);
      Console.ReadKey();
    }
  }
}
