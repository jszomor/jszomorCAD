using JsonFindKey;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EquipmentPosition
{
  public static class EquipmentSelector
  {
    public static double AvgFlowCalc()
    {
      SelectorProperty selectorProperty = new SelectorProperty();

      selectorProperty.AvgDailyFlow = Convert.ToInt32(JsonProcessClass.JsonProcessValue("Q_inf_AA"));

      selectorProperty.AvgHourlyFlow = selectorProperty.AvgDailyFlow / 24;

      return selectorProperty.AvgHourlyFlow;
    }

    public static double EqPumpNumberSelect(this SelectorProperty selectorProperty)
    {
      double avgH = AvgFlowCalc();
      try
      {
        if (AvgFlowCalc() <= 800)
        {
          selectorProperty.NumberOfEqipment = 1;
        }
        else
        {
            if      (avgH <= 1200)
              selectorProperty.Capacity = 800;
            else if (avgH <= 2000)
              selectorProperty.Capacity = 1000;
            else if (avgH <= 4000)
              selectorProperty.Capacity = 1200;
            else if (avgH <= 6000)
              selectorProperty.Capacity = 1500;
            else if (avgH <= 8500)
              selectorProperty.Capacity = 2000;
          
          selectorProperty.NumberOfEqipment = Convert.ToInt32(AvgFlowCalc() / selectorProperty.Capacity);          
        }        
      }
      catch (ArgumentOutOfRangeException ex)
      {
        throw new ArgumentOutOfRangeException($"Avg flow is out of range,{ex}");
      }
      return selectorProperty.NumberOfEqipment;      
    }

    public static double EqPumpFlowCalc(this SelectorProperty selectorProperty) => AvgFlowCalc() / EqPumpNumberSelect(selectorProperty);

    public static double WaterLevelCalc(SelectorProperty selectorProperty) => AvgFlowCalc() / 3600 / selectorProperty.FlowSpeed * 1000000 / selectorProperty.NumberOfTrain;

    public static int ChannelGateWidthSelect(this SelectorProperty selectorProperty)
    {
      double avgH = AvgFlowCalc();
      try
      {
          if      (avgH <= 900)
            selectorProperty.ChannelWidth = 800;
          else if (avgH <= 1900)
            selectorProperty.ChannelWidth = 1000;
          else if (avgH <= 3900)
            selectorProperty.ChannelWidth = 1200;
          else if (avgH <= 5900)
            selectorProperty.ChannelWidth = 1400;
          else if (avgH <= 7900)
            selectorProperty.ChannelWidth = 1600;
          else if (avgH <= 9900)
            selectorProperty.ChannelWidth = 1800;
          else if (avgH <= 11900)
            selectorProperty.ChannelWidth = 2000;
        
        return selectorProperty.ChannelWidth;
      }
      catch (ArgumentOutOfRangeException ex)
      {
        throw new ArgumentOutOfRangeException($"Avg flow is out of range,{ex}");
      }
    }    

    public static double ChannelGateHeightSelect(SelectorProperty selectorProperty)
    {
      double devided = (WaterLevelCalc(selectorProperty) / ChannelGateWidthSelect(selectorProperty)) + selectorProperty.FreeBoard;
      double result = Convert.ToInt32(devided / 100) * 100;
      return result;     
    }
  }
}
