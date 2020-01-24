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
  public class BlockDeserialize
  {
    public JsonPID ReadJsonData() => ReadJsonData(@"E:\Jszomor\source\repos\jszomorCAD\jCAD.PID_Builder\JsonStringBuilder.json");

    public JsonPID ReadJsonData(string fileName)
    {
      string jsonStringText = System.IO.File.ReadAllText(fileName);

      return JsonConvert.DeserializeObject<JsonPID>(jsonStringText);
    }
  }
}
