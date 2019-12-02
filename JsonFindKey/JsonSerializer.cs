using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace JsonFindKey
{
  public class JsonSerializer
  {
    public void JsonSeri(object item)
    {
      string fileJson = @"\fileJson.json";
      string dirPath = @"C:\Users\jszomor\Google Drive\Programozas\Practice"; //work
      //string dirPath = @"C:\Users\JANO\Google Drive\Programozas\Practice"; //home
      string filePath = dirPath + fileJson;

      var json = JsonConvert.SerializeObject(item, Formatting.Indented);
      System.IO.File.WriteAllText(filePath, json); 
    }

    private static StreamWriter StreamWriteMethod(string file)
    {
      string dirPath = @"C:\Users\jszomor\Google Drive\Programozas\Practice"; //work
      //string dirPath = @"C:\Users\JANO\Google Drive\Programozas\Practice"; //home
      string filePath = dirPath + file;
      // ha nem létezik a könyvtár
      if (Directory.Exists(dirPath) == false)
      {
        // akkor elkészítjük
        Directory.CreateDirectory(dirPath);
      }
      FileInfo fi = new FileInfo(filePath);
      StreamWriter streamWriteOutput = fi.CreateText();
      return streamWriteOutput;
    }
  }
}
