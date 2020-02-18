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
      SelectorProperty selectorProperty = new SelectorProperty();

      var pumpIndex = JsonClass.JsonEquipmentValue("Equalization Tank Pump");
      var q_inf_aa = JsonProcessClass.JsonProcessValue("Q_inf_AA");
      var numberOfPump = EquipmentSelector.EqPumpNumberSelect(selectorProperty);

      Assert.AreEqual(22L, pumpIndex);
      //Assert.AreEqual(200000, q_inf_aa);
      //Assert.AreEqual(13, numberOfPump);
      Console.WriteLine(q_inf_aa);
      Console.WriteLine(numberOfPump);
    }

    [TestMethod]
    public void ChannelTest()
    {
      var selectorProperty = new SelectorProperty();

      //EquipmentSelector.AvgFlowCalc(selectorProperty);
      selectorProperty.AvgDailyFlow = Convert.ToInt32(JsonProcessClass.JsonProcessValue("Q_inf_AA"));
      selectorProperty.NumberOfTrain = Convert.ToInt32(JsonProcessClass.JsonProcessValue("NTS_FCR"));
      //var q_inf_aa = JsonProcessClass.JsonProcessValue("Q_inf_AA");
      Console.WriteLine($"ChannelHeight:{EquipmentSelector.ChannelGateHeightSelect(selectorProperty)}");
      Console.WriteLine($"ChannelWidth:{EquipmentSelector.ChannelGateWidthSelect(selectorProperty)}");
    }
  }
}
