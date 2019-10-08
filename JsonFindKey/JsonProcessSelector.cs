using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace JsonFindKey
{
  public class JsonProcessClass
  {
    public static long JsonProcessValue(string processName)
    {
      var jsonStringProcess = System.IO.File.ReadAllText(@"C:\Users\jszomor\source\repos\jszomorCAD\FindJsonKey\process.json");

      var jsonDictProcess = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonStringProcess);
      long searchedValueProcess = 0;
      try
      {    
        searchedValueProcess = DictSearcher.GetValueByKey(jsonDictProcess, processName);
        return searchedValueProcess;
      }
      catch (ArgumentException ex)
      {
        Console.WriteLine(ex.Message);
      }
      throw new ArgumentException($"seached key {searchedValueProcess}'s value is not found");
    }
  }
}
