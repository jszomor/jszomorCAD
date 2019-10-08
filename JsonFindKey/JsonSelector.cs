using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace JsonFindKey
{
  public static class JsonClass
  {
    public static void JsonSelector(string processName, string equipmentName)
    {
      var jsonString = System.IO.File.ReadAllText(@"C:\Users\jszomor\source\repos\jszomorCAD\FindJsonKey\Equipments.json");
      var jsonStringProcess = System.IO.File.ReadAllText(@"C:\Users\jszomor\source\repos\jszomorCAD\FindJsonKey\process.json");

      var jsonDict = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonString);
      var jsonDictProcess = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonStringProcess);

      try
      {
        var searchedValue = DictSearcher.GetValueByKey(jsonDict, equipmentName);
        var searchedValueProcess = DictSearcher.GetValueByKey(jsonDictProcess, processName);

        Console.WriteLine($"{ searchedValue} { equipmentName}");
      }
      catch (ArgumentException ex)
      {
        Console.WriteLine(ex.Message);
      }
      Console.WriteLine("Hello World!");
    }
  }
}
