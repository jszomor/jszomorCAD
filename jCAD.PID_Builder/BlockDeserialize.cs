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
    //public object SearchedValue { get; set; }

    public JsonPID ReadJsonData() => ReadJsonData(@"C:\Users\jszomor\source\repos\jszomorCAD\jCAD.PID_Builder\JsonStringBuilder.json");

    public JsonPID ReadJsonData(string fileName)
    {
      string jsonStringText = System.IO.File.ReadAllText(fileName);

      return JsonConvert.DeserializeObject<JsonPID>(jsonStringText);
    }
  }
  //  public object BlockSearch(string equipmentName)
  //  {
  //    string jsonStringText = System.IO.File.ReadAllText(@"C:\Users\jszomor\source\repos\jszomorCAD\jCAD.PID_Builder\JsonStringBuilder.json");

  //    var result = JsonConvert.DeserializeObject<JsonPID>(jsonStringText);

  //    //JObject jsonObj = JObject.Parse(jsonStringText);
  //    //JToken jsonToken = jsonObj["Blocks"];
  //    ////string jsonString = jsonToken.ToString();
  //    //Type targetType = typeof(JToken);
  //    //System.Convert.ChangeType(jsonToken.ToString(), targetType);
  //    //// get JSON result objects into a list
  //    ////IList<string> results = jsonString["Blocks"].Select(t => (string)t).ToList();
  //    //////IList<JToken> results = jsonString["Blocks"][0].Children().ToList();
  //    ////// serialize JSON results into .NET objects
  //    //////IList<JsonBlockProperty> searchResults = new List<JsonBlockProperty>();
  //    ////foreach (string result in results)
  //    ////{
  //    ////  //Type type = typeof(string);
  //    ////  //var i1 = System.Convert.ChangeType(result.ToString(), type);
  //    ////  //var i3 = Convert.ToString(i1);

  //    ////  JsonDeser(result, equipmentName);
  //    ////  // JToken.ToObject is a helper method that uses JsonSerializer internally
  //    ////  //JsonBlockProperty searchResult = result.ToObject<JsonBlockProperty>();
  //    ////  //searchResults.Add(searchResult);
  //    ////}

  //    //JsonDeser(jsonToken, equipmentName);

  //    return SearchedValue;
  //  }

  //  public object JsonDeser(string jsonString, string variableName)
  //  {
  //    //var obj = (JObject)JsonConvert.DeserializeObject(jsonString);
  //    //Type type = typeof(string);
  //    //var i1 = System.Convert.ChangeType(obj["Blocks"].ToString(), type);
  //    //var i3 = Convert.ToString(i1);
  //    //var i2 = JsonConvert.DeserializeObject(obj["id"].ToString(), type);

  //    var jsonDict = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonString);
  //    //dynamic obj = JsonConvert.DeserializeObject(jsonString);
  //    //var blocksJson = JsonConvert.DeserializeObject<Dictionary<string, object>>(JsonConvert.SerializeObject(obj["Blocks"]));
  //    try
  //    {
  //      SearchedValue = GetValueByKey(jsonDict, variableName);
  //      return SearchedValue;
  //    }
  //    catch (ArgumentException ex)
  //    {
  //      Console.WriteLine(ex.Message);
  //    }

  //    throw new ArgumentException($"seached key {SearchedValue} value is not found");
  //  }
 
  //  public static object GetValueByKey(Dictionary<string, object> inputDict, string searchedKey)
  //  {
  //    foreach (var key in inputDict.Keys)
  //    {
  //      if (key == searchedKey && !string.IsNullOrEmpty(key))
  //      {
  //        var seachedValue = inputDict[key];

  //        if (seachedValue is object returnValue)
  //        {
  //          return returnValue;
  //        }

  //        else
  //        {
  //          throw new ArgumentException($"seached key {searchedKey}'s value is not an integer");
  //        }
  //      }

  //      else
  //      {
  //        var currentValue = inputDict[key];

  //        var currentObject = currentValue as JObject;

  //        try
  //        {
  //          var currentDict = currentObject.ToObject<Dictionary<string, object>>();
  //          if (currentDict != null)
  //          {
  //            try
  //            {
  //              return GetValueByKey(currentDict, searchedKey);

  //            }
  //            catch (ArgumentException e)
  //            {
  //              Console.WriteLine(e.Message);
  //              continue;
  //            }
  //          }
  //        }

  //        catch (NullReferenceException)
  //        {
  //        }
  //      }
  //    }
  //    throw new ArgumentException($"seached key {searchedKey}'s value is not found");
  //  }
  //}
}
