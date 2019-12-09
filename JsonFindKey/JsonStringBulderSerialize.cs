using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace JsonFindKey
{
  public class JsonStringBuilderSerialize
  {
    public void StringBuilderSerialize(JsonStringBuilderProperty properties)
    {
      StringWriter sw = new StringWriter();
      JsonTextWriter writer = new JsonTextWriter(sw);

      writer.Formatting = Formatting.Indented;     

      //properties.TagX = 2;
      //properties.TagY = 1;
      //properties.BlockX = 35;
      //properties.BlockY = 22;
      
      writer.Formatting = Formatting.Indented;

      writer.WriteStartObject();
      writer.WritePropertyName("Name");
      writer.WriteValue(properties.BlockName);
      writer.WritePropertyName(properties.VisibilityName);
      writer.WriteValue(properties.VisibilityValue);
      writer.WritePropertyName("Position");
      writer.WriteStartArray();
      writer.WriteValue(properties.BlockX);
      writer.WriteValue(properties.BlockY);
      writer.WriteEndArray();
      writer.WritePropertyName("TagPosition");
      writer.WriteStartArray();
      writer.WriteValue(Math.Round(properties.TagX, 2));
      writer.WriteValue(Math.Round(properties.TagY, 2));
      writer.WriteEndArray();
      writer.WriteEndObject();
      writer.Flush();

      string jsonText = sw.GetStringBuilder().ToString();

      //Console.WriteLine(jsonText);

      string fileJson = @"\JsonStringBuilder.json";
      string dirPath = @"C:\Users\jszomor\Google Drive\Programozas\Practice"; //work
      //string dirPath = @"C:\Users\JANO\Google Drive\Programozas\Practice"; //home

      string filePath = dirPath + fileJson;

      File.WriteAllText(filePath, jsonText);

      //Console.ReadKey();
    }
  }
}
