using JsonFindKey;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace JsonFindKey
{
  public static class JsonClass
  {
    public static long JsonEquipmentValue(string equipmentName)
    {
      var jsonString = System.IO.File.ReadAllText(@"C:\Users\jszomor\source\repos\jszomorCAD\FindJsonKey\Equipments.json");

      var jsonDict = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonString);
      long searchedValue = 0;
      try
      {
        searchedValue = DictSearcher.GetValueByKey(jsonDict, equipmentName);
        return searchedValue;
      }
      catch (ArgumentException ex)
      {
        Console.WriteLine(ex.Message);
      }
      
      throw new ArgumentException($"seached key {searchedValue}'s value is not found");
    }
  }
}
