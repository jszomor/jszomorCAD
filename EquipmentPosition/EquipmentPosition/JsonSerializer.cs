using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using EquipmentPosition;
using JsonParse;
using Newtonsoft.Json;

namespace EquipmentPosition
{
  public class JsonWrite2
  {
    public void JsonSeri2(SerializationProperty serialization)
    {
      string fileJson = @"\fileJson.json";
      string dirPath = @"C:\Users\jszomor\Google Drive\Programozas\Practice"; //work
      //string dirPath = @"C:\Users\JANO\Google Drive\Programozas\Practice"; //home

      string filePath = dirPath + fileJson;
      var serializer = new JsonSerializer();
      serializer.Formatting = Formatting.Indented;
      using (StreamWriter sw = new StreamWriter(filePath))
      {
        using (JsonWriter writer = new JsonTextWriter(sw))
        {
          serializer.Serialize(writer, serialization);
        }
      }
    }
  }
}
