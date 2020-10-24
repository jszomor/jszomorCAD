using JsonFindKey;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace JsonFindKey
{
  public class JsonClass
  {    
    public static long JsonEquipmentValue(string equipmentName)
    {    
      var jsonString = System.IO.File.ReadAllText(@"C:\Users\jszom\source\repos\jszomorCAD\FindJsonKey\Equipments.json");
      
      var jsonDeser = new JsonDeserializer();
      jsonDeser.JsonDeser(jsonString, equipmentName);

      return jsonDeser.SearchedValue;

      #region old code
      //var jsonDict = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonString);
      //long searchedValue = 0;
      //try
      //{
      //  searchedValue = DictSearcher.GetValueByKey(jsonDict, equipmentName);
      //  return searchedValue;
      //}
      //catch (ArgumentException ex)
      //{
      //  Console.WriteLine(ex.Message);
      //}

      //throw new ArgumentException($"seached key {searchedValue}'s value is not found");
      #endregion
    }
  }
}
