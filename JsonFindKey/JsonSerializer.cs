using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using JsonParse;
using Newtonsoft.Json;

namespace JsonFindKey
{
  public class JsonWrite
  {
    public void JsonSeri(JsonBlockProperty jsonBlockProperty)
    {
      string fileJson = @"\fileJson.json";
      string dirPath = @"E:\Jszomor\Google Drive\Programozas\Practice"; //work
      //string dirPath = @"C:\Users\JANO\Google Drive\Programozas\Practice"; //home

      string filePath = dirPath + fileJson;
      var serializer = new JsonSerializer();
      serializer.Formatting = Formatting.Indented;
      using (StreamWriter sw = new StreamWriter(filePath))
      {
        using (JsonWriter writer = new JsonTextWriter(sw))
        {
          serializer.Serialize(writer, jsonBlockProperty);
        }
      }
    }
  }
}
