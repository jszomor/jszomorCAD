﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using jCAD.PID_Builder;
using JsonParse;
using static jCAD.Test.TestProperty;

namespace jCAD.Test
{
  [TestClass]
  public class JsonCompare
  {
    [TestMethod]
    public void JsonTest()
    {
      string jsonInput = @"E:\Jszomor\source\repos\jszomorCAD\jCAD.PID_Builder\JsonPIDBuild.json";
      var input = System.IO.File.ReadAllLines(jsonInput);

      //foreach (var item in input)
      //{
      //  Console.WriteLine(item);
      //}

      string jsonOutput = @"E:\Jszomor\source\repos\jszomorCAD\jCAD.PID_Builder\JsonPIDBuildCopy.json";
      var output = System.IO.File.ReadAllLines(jsonOutput);
      
      if(output.Length == input.Length)
      {
        //Assert.IsTrue(true);
        for (int i = 0; i < input.Length; i++)
        {
          Assert.AreEqual(input[i], output[i]);
        }
      }
      else
      {
        Assert.IsTrue(false);
      }
    }

    [TestMethod]
    public void HashCompare()
    {
      string jsonInput = @"E:\Jszomor\source\repos\jszomorCAD\jCAD.PID_Builder\JsonPIDBuild.json";
      string jsonOutput = @"E:\Jszomor\source\repos\jszomorCAD\jCAD.PID_Builder\JsonPIDBuildCopy.json";

      var inputFileInfo = new System.IO.FileInfo(jsonInput);
      var outputFileInfo = new System.IO.FileInfo(jsonOutput);
      if (inputFileInfo.Length == outputFileInfo.Length && inputFileInfo.Length > 0)
      {
        byte[] hash1, hash2;
        using (HashAlgorithm ha = HashAlgorithm.Create())
        {
          using (FileStream f1 = new FileStream(jsonInput, FileMode.Open))
          {
            using (FileStream f2 = new FileStream(jsonOutput, FileMode.Open))
            {
              /* Calculate Hash */
              hash1 = ha.ComputeHash(f1);
              hash2 = ha.ComputeHash(f2);
            }
          }
        }
        /* Show Hash in TextBoxes */
        var jsonInputHash = BitConverter.ToString(hash1);
        var jsonOutputHash = BitConverter.ToString(hash2);
        /* Compare the hash and Show Message box */
        Assert.AreEqual(jsonInputHash, jsonOutputHash);
        if (jsonInputHash == jsonOutputHash)
        {
          MessageBox.Show("Files are Equal !");
        }
        else
        {
          MessageBox.Show("Files are Different !");
        }
      }
      else
      {
        Assert.IsTrue(false);

        MessageBox.Show("Files have different length!");
      }
    }

    [TestMethod]
    public void FilesAreEqual_Hash()
    {
      FileInfo first = new FileInfo(@"E:\Jszomor\source\repos\jszomorCAD\jCAD.PID_Builder\JsonPIDBuild.json");
      FileInfo second = new FileInfo(@"E:\Jszomor\source\repos\jszomorCAD\jCAD.PID_Builder\JsonPIDBuildCopy.json");
      byte[] firstHash = MD5.Create().ComputeHash(first.OpenRead());
      byte[] secondHash = MD5.Create().ComputeHash(second.OpenRead());

      for (int i = 0; i < firstHash.Length; i++)
      {
        if (firstHash[i] != secondHash[i])
          Assert.IsTrue(false);
      }
      Assert.IsTrue(true);
    }

    [TestMethod]
    public void DeepComparer()
    {
      var fileName1 = @"E:\Jszomor\source\repos\jszomorCAD\jCAD.PID_Builder\JsonPIDBuild.json";
      //var fileName1 = @"C:\Users\JANO\source\repos\jszomorCAD\jCAD.PID_Builder\JsonPIDBuild.json"; //DELL
      var fileName2 = @"E:\Jszomor\source\repos\jszomorCAD\jCAD.PID_Builder\JsonPIDBuildCopy.json";
      //var fileName2 = @"C:\Users\JANO\source\repos\jszomorCAD\jCAD.PID_Builder\JsonPIDBuildCopy.json"; //DELL
      var blockDeserialize = new BlockDeserializer();
      var jsonPID1 = blockDeserialize.ReadJsonData(fileName1);
      var jsonPID2 = blockDeserialize.ReadJsonData(fileName2);
      var dictBlock1 = new Dictionary<int, JsonBlockProperty>();
      var dictBlock2 = new Dictionary<int, JsonBlockProperty>();

      if (jsonPID1.Blocks.Count == jsonPID2.Blocks.Count)
      {
        for (int i = 0; i < jsonPID1.Blocks.Count; i++)
        {
          dictBlock1.Add(jsonPID1.Blocks[i].Attributes.Internal_Id, jsonPID1.Blocks[i]);
          dictBlock2.Add(jsonPID2.Blocks[i].Attributes.Internal_Id, jsonPID2.Blocks[i]);
        }
      }
      foreach (var i in dictBlock1)
      {
        if(dictBlock2.TryGetValue(i.Key, out JsonBlockProperty compareValue))
        {
          DeepEx.AttributesCompare(i.Value, compareValue);
        }
      }

      if(jsonPID1.Blocks.Count == jsonPID2.Blocks.Count)
      {
        for (int i = 0; i < jsonPID1.Blocks.Count; i++)
        {
          for (int j = 0; j < jsonPID2.Blocks.Count; j++)
          {
            var intId1 = jsonPID1.Blocks[i].Attributes.Internal_Id;
            var intId2 = jsonPID2.Blocks[j].Attributes.Internal_Id;

            if (intId1 == intId2)
            {
              DeepEx.AttributesCompare(jsonPID1.Blocks[i], jsonPID2.Blocks[j]);
            }
          } 
        }
      }
    }
  }
}
