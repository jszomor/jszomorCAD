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
      var pumpIndex = JsonClass.JsonEquipmentValue("Equalization Tank Pump");
      var q_inf_aa = JsonProcessClass.JsonProcessValue("Q_inf_AA");
      var numberOfPump = EquipmentSelector.EqPumpSelect();

      Assert.AreEqual(22L, pumpIndex);
      //Assert.AreEqual(200000, q_inf_aa);
      //Assert.AreEqual(13, numberOfPump);
      Console.WriteLine(q_inf_aa);
      Console.WriteLine(numberOfPump);
    }
  }
}
