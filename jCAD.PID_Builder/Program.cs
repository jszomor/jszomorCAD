using Autodesk.AutoCAD.DatabaseServices;
using JsonParse;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace jCAD.PID_Builder
{
  public class Program
  {

    static void Main(string[] args)
    {

      FileInfo jsonInput = new FileInfo(@"E:\Jszomor\source\repos\jszomorCAD\jCAD.PID_Builder\JsonPIDBuild.json");
      FileInfo jsonOutput = new FileInfo(@"E:\Jszomor\source\repos\jszomorCAD\jCAD.PID_Builder\JsonPIDBuildCopy.json");
      bool isEqual = FilesAreEqual_Hash(jsonInput, jsonOutput);

      // Specify the data source.
      //int[] scores = new int[] { 97, 92, 81, 60 };

      //// Define the query expression.
      //IEnumerable<int> scoreQuery =
      //    from score in scores
      //    where score > 80
      //    select score;

      //// Execute the query.
      //foreach (int i in scoreQuery)
      //{
      //  Console.Write(i + " ");
      //}

      //Console.ReadKey();
    }

    static bool FilesAreEqual_Hash(FileInfo first, FileInfo second)
    {

      byte[] firstHash = MD5.Create().ComputeHash(first.OpenRead());
      byte[] secondHash = MD5.Create().ComputeHash(second.OpenRead());

      for (int i = 0; i < firstHash.Length; i++)
      {
        if (firstHash[i] != secondHash[i])
          return false;
      }
      return true;
    }
  }
}
