using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace FindJsonKey
{
  class Program
  {
    static void Main(string[] args)
    {
      var jsonString = System.IO.File.ReadAllText(@"C:\Users\jszomor\source\repos\jszomorCAD\FindJsonKey\Equipments.json");


      var jsonDict = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonString);

      try
      {
        var seachedValue = DictSearcher.GetValueByKey(jsonDict, "Grease pump");
        Console.WriteLine(seachedValue);

      }
      catch (ArgumentException ex)
      {
        Console.WriteLine(ex.Message);
        
      }


      Console.WriteLine("Hello World!");
    }


  }
}
