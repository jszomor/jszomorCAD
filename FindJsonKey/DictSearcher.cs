using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace FindJsonKey
{
  class DictSearcher
  {
    public static long GetValueByKey(Dictionary<string, object> inputDict, string searchedKey)
    {
      foreach (var key in inputDict.Keys)
      {
        if (key == searchedKey)
        {
          var seachedValue = inputDict[key];

          if (seachedValue is long returnValue)
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
              catch(ArgumentException e)
              {
                continue;
              }
            }
          }

          catch(NullReferenceException)
          {
          }
        }
      }
      throw new ArgumentException($"seached key {searchedKey}'s value is not found");
    }
  }
}
