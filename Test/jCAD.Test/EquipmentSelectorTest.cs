using System;
using EquipmentPosition;
using JsonFindKey;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace jCAD.Test
{
  [TestClass]
  public class EquipmentSelectorTest
  {
    [TestMethod]
    public void EqPumpSelectTest()
    {
      
      Console.WriteLine (JsonClass.JsonEquipmentValue("Equalization Tank Pump"));
      Console.WriteLine (JsonProcessClass.JsonProcessValue("Q_inf_AA"));
      Console.WriteLine(EquipmentSelector.EqPumpSelect());
      
    }
  }
}
