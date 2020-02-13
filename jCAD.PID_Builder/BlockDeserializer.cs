using JsonFindKey;
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
  public class BlockDeserializer
  {
    //public JsonPID ReadJsonData() => ReadJsonData(@"E:\Jszomor\source\repos\jszomorCAD\jCAD.PID_Builder\JsonPIDBuild.json");

    public JsonPID ReadJsonData(string fileName)
    {
      string jsonStringText = System.IO.File.ReadAllText(fileName);
      var result = JsonConvert.DeserializeObject<JsonPID>(jsonStringText);
      result.Lines.Sort();

      return result;
    }
  }
}
