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
    public object SearchedValue { get; set; }

    public object BlockSearch(string equipmentName)
    {
      string jsonString = System.IO.File.ReadAllText(@"C:\Users\jszomor\source\repos\jszomorCAD\jCAD.PID_Builder\JsonStringBuilder.json");

      JsonDeser(jsonString, equipmentName);

      return SearchedValue;
    }

    public object JsonDeser(string jsonString, string variableName)
    {

      var jsonDict = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonString);
      try
      {
        SearchedValue = GetValueByKey(jsonDict, variableName);
        return SearchedValue;
      }
      catch (ArgumentException ex)
      {
        Console.WriteLine(ex.Message);
      }

      throw new ArgumentException($"seached key {SearchedValue} value is not found");
    }
 
    public static object GetValueByKey(Dictionary<string, object> inputDict, string searchedKey)
    {
      foreach (var key in inputDict.Keys)
      {
        if (key == searchedKey)
        {
          var seachedValue = inputDict[key];

          if (seachedValue is object returnValue)
          {
            return returnValue;
          }

          else
          {
            throw new ArgumentException($"seached key {searchedKey}'s value is not an integer");
          }
        }

        else
        {
          var currentValue = inputDict[key];

          var currentObject = currentValue as JObject;

          try
          {
            var currentDict = currentObject.ToObject<Dictionary<string, object>>();
            if (currentDict != null)
            {
              try
              {
                return GetValueByKey(currentDict, searchedKey);

              }
              catch (ArgumentException e)
              {
                Console.WriteLine(e.Message);
                continue;
              }
            }
          }

          catch (NullReferenceException)
          {
          }
        }
      }
      throw new ArgumentException($"seached key {searchedKey}'s value is not found");
    }
  }
}
