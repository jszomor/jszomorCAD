using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace jCAD.Test
{
  [TestClass]
  public class TruthTableTest
  {
    [TestMethod]
    public void TestMethod1()
    {
      bool p, q;

      Console.WriteLine("  P    \t Q \t AND \t OR\t XOR\t NOT");

      p = true; q = true;
      var case1 = (p + "\t" + q + "\t" + (p & q) + "\t" + (p | q) + "\t" + (p ^ q) + "\t" + (!p));
      Console.WriteLine(case1);
      //    Console.Write((p&q) + "\t" + (p|q) + "\t"); 
      //    Console.WriteLine((p^q) + "\t" + (!p)); 

      p = true; q = false;
      Console.Write(p + "\t" + q + "\t");
      Console.Write((p & q) + "\t" + (p | q) + "\t");
      Console.WriteLine((p ^ q) + "\t" + (!p));

      p = false; q = true;
      Console.Write(p + "\t" + q + "\t");
      Console.Write((p & q) + "\t" + (p | q) + "\t");
      Console.WriteLine((p ^ q) + "\t" + (!p));

      p = false; q = false;
      Console.Write(p + "\t" + q + "\t");
      Console.Write((p & q) + "\t" + (p | q) + "\t");
      Console.WriteLine((p ^ q) + "\t" + (!p));
    }

    public bool isChemicalPrecipitation;
    public bool isChemicalFlocc1;
    public bool isClarifier;
    public int nunits_rapid_mixer;
    public bool isChemicalFlocc2;
    public bool isClarifierAndDiscfilter;
    public bool isDiscfiter;
    public bool RAS;
    public int SludgeLines;
      
    [TestMethod]
    public void VariableArrayTest()
    {
      var variableArray = new bool[7] {isChemicalPrecipitation = true, isChemicalFlocc1, isClarifier, isChemicalFlocc2, isClarifierAndDiscfilter, isDiscfiter,RAS};

      for (int i = 0; i < variableArray.Length; i++)
      {
        var variableArrayString = Convert.ToString(variableArray[i]);
        Console.WriteLine(variableArrayString + variableArray[i]);
      }
      



    }
  }
}
