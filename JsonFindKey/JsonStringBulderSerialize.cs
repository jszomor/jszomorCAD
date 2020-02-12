using JsonParse;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace JsonFindKey
{
  public class JsonStringBuilderSerialize
  {
    public void StringBuilderSerialize(JsonPID jsonPID, string fileName)
    {
      //string fileJson = "JsonStringBuilder.json";
      string dirPath = @"E:\Jszomor\source\repos\jszomorCAD\jCAD.PID_Builder\"; //work

      var path = System.IO.Path.Combine(dirPath, fileName);

      var serializer = new JsonSerializer
      {
        NullValueHandling = NullValueHandling.Ignore,
        //DefaultValueHandling = DefaultValueHandling.Ignore,
        Formatting = Formatting.Indented
      };
      using (StreamWriter sw = new StreamWriter(path))
      {
        using (JsonWriter writer = new JsonTextWriter(sw))
        {
          serializer.Serialize(writer, jsonPID);
        }
      }
      return;
    }
  }
}
