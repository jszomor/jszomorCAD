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
    public void StringBuilderSerialize(IEnumerable<JsonBlockProperty> properties)
    {
      string fileJson = "JsonStringBuilder.json";
      string dirPath = @"C:\Users\jszomor\Google Drive\Programozas\Practice"; //work

      var fileName = System.IO.Path.Combine(dirPath, fileJson);

      var serializer = new JsonSerializer();
      serializer.NullValueHandling = NullValueHandling.Ignore;
      serializer.Formatting = Formatting.Indented;
      using (StreamWriter sw = new StreamWriter(fileName))
      {
        using (JsonWriter writer = new JsonTextWriter(sw))
        {
          serializer.Serialize(writer, properties);
        }
      }
      return;
    }
  }
}
