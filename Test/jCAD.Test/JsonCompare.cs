using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jCAD.Test
{
  [TestClass]
  public class JsonCompare
  {
    [TestMethod]
    public void JsonTest()
    {
      string jsonInput = @"D:\Jszomor\source\repos\jszomorCAD\jCAD.PID_Builder\JsonStringBuilderOrig.json";
      var input = System.IO.File.ReadAllLines(jsonInput);
      foreach (var item in input)
      {
        Console.WriteLine(item);
      }

      string jsonOutput = @"D:\Jszomor\source\repos\jszomorCAD\jCAD.PID_Builder\JsonStringBuilder.json";
      var output = System.IO.File.ReadAllLines(jsonOutput);
      for (int i = 0; i < input.Length; i++)
      {
        Assert.AreEqual(input[i], output[i]);
      }

    }
  }
}
