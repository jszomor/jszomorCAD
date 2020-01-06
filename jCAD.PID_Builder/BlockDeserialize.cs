using JsonFindKey;
using JsonParse;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jCAD.PID_Builder
{
  public class BlockDeserialize
  {
    public string BlockSearch (string equipmentName)
    {
      string jsonString = System.IO.File.ReadAllText(@"C:\Users\jszomor\source\repos\jszomorCAD\jCAD.PID_Builder\JsonStringBuilder.json");

      //var jsonBlockProperty = JsonBlockProperty();

      JsonBlockProperty jsonBlockProperty = JsonConvert.DeserializeObject<JsonBlockProperty>(jsonString);

      //System.Diagnostics.Debug.Print($"BlockName: {jsonBlockProperty.Misc.BlockName}");

      return jsonBlockProperty.Misc.BlockName;
      //JsonDeserializer jsonDeser = new JsonDeserializer();
      //jsonDeser.JsonDeser(jsonString, equipmentName);

      //return jsonDeser.SearchedValue;
    }
  }
}
