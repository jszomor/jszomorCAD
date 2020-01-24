using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace JsonFindKey
{
  public class JsonDeserializer
  {
    public long SearchedValue { get; set; }

    public long JsonDeser(string jsonString, string variableName)
    {

      var jsonDict = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonString);
      try
      {
        SearchedValue = DictSearcher.GetValueByKey(jsonDict, variableName);
        return SearchedValue;
      }
      catch (ArgumentException ex)
      {
        Console.WriteLine(ex.Message);
      }

      throw new ArgumentException($"seached key {SearchedValue} value is not found");
    }
  }
}
