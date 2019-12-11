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
      var serializer = new JsonSerializer();

      serializer.NullValueHandling = NullValueHandling.Ignore;
      serializer.Formatting = Formatting.Indented;

      StringWriter sw = new StringWriter();
      JsonTextWriter writer = new JsonTextWriter(sw);

      writer.Formatting = Formatting.Indented;

      //var precTagX = Math.Round(properties.TagX,2);
      //var precTagY = Math.Round(properties.TagY, 2);

      writer.WriteStartObject();
      writer.WriteComment($"**************************");
      writer.WritePropertyName("BlockName");
      writer.WriteValue(properties.BlockName);
      writer.WritePropertyName("Layer");
      writer.WriteValue(properties.LayerName);
      writer.WritePropertyName(properties.VisibilityName);
      writer.WriteValue(properties.VisibilityValue);
      writer.WritePropertyName("ItemPosition");
      writer.WriteStartArray();
      writer.WriteValue($"Position X: {properties.BlockX}");
      writer.WriteValue($"Position Y: {properties.BlockY}");
      writer.WriteValue($"Rotation: {properties.Rotation}");
      writer.WriteEndArray();
      writer.WritePropertyName("Custom");
      writer.WriteStartArray();
      writer.WriteValue($"Position X: {properties.TagX}");
      writer.WriteValue($"Position Y: {properties.TagY}");
      writer.WriteValue($"Flip state: {properties.FlipState}");
      writer.WriteValue($"Angle: {properties.Angle}");
      writer.WriteValue($"Distance: {properties.Distance}");
      writer.WriteValue($"Distance1: {properties.Distance1}");
      writer.WriteValue($"Distance2: {properties.Distance2}");
      writer.WriteValue($"Distance3: {properties.Distance3}");
      writer.WriteValue($"Distance4: {properties.Distance4}");
      writer.WriteValue($"Distance5: {properties.Distance5}");
      writer.WriteEndArray();
      writer.WritePropertyName("VFDPosition");
      writer.WriteStartArray();
      writer.WriteValue($"Angle1: {properties.Angle1}");
      writer.WriteValue($"Angle2: {properties.Angle2}");
      writer.WriteEndArray();
      writer.WriteEndObject();
      writer.Flush();

      string jsonText = sw.GetStringBuilder().ToString();
      string fileJson = @"\JsonStringBuilder.json";
      string dirPath = @"C:\Users\jszomor\Google Drive\Programozas\Practice"; //work
      //string dirPath = @"C:\Users\JANO\Google Drive\Programozas\Practice"; //home

      string filePath = dirPath + fileJson;

      //var info = new FileInfo(filePath);
      //if ((!info.Exists) || info.Length == 0)
      //{
      File.AppendAllText(filePath, jsonText);
      //}
      //else
      //  File.WriteAllText(filePath, jsonText);
    }
  }
}
