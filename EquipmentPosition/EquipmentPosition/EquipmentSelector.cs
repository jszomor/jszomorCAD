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
      try
      {
        if (AvgFlowCalc() <= 800)
        {
          selectorProperty.NumberOfEqipment = 1;
        }
        else
        {
            if (AvgFlowCalc() >= 801 && AvgFlowCalc() <= 1200)
              selectorProperty.Capacity = 800;
            else if (AvgFlowCalc() >= 1201 && AvgFlowCalc() <= 2000)
              selectorProperty.Capacity = 1000;
            else if (AvgFlowCalc() >= 2001 && AvgFlowCalc() <= 4000)
              selectorProperty.Capacity = 1200;
            else if (AvgFlowCalc() >= 4001 && AvgFlowCalc() <= 6000)
              selectorProperty.Capacity = 1500;
            else if (AvgFlowCalc() >= 6001 && AvgFlowCalc() <= 8500)
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
      try
      {
          if (AvgFlowCalc() <= 900)
            selectorProperty.ChannelWidth = 600;
          else if (AvgFlowCalc() >= 901 && AvgFlowCalc() <= 1900)
            selectorProperty.ChannelWidth = 1000;
          else if (AvgFlowCalc() >= 1901 && AvgFlowCalc() <= 3900)
            selectorProperty.ChannelWidth = 1200;
          else if (AvgFlowCalc() >= 3901 && AvgFlowCalc() <= 5900)
            selectorProperty.ChannelWidth = 1500;
          else if (AvgFlowCalc() >= 5901 && AvgFlowCalc() <= 7900)
            selectorProperty.ChannelWidth = 1600;
          else if (AvgFlowCalc() >= 7901 && AvgFlowCalc() <= 9900)
            selectorProperty.ChannelWidth = 1800;
          else if (AvgFlowCalc() >= 9901 && AvgFlowCalc() <= 13900)
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
      try
      {
          if (AvgFlowCalc() <= 1900)
            selectorProperty.ChannelHeight = 800;
          else if (AvgFlowCalc() >= 1901 && AvgFlowCalc() <= 3900)
            selectorProperty.ChannelHeight = 1200;
          else if (AvgFlowCalc() >= 3901 && AvgFlowCalc() <= 5900)
            selectorProperty.ChannelHeight = 1500;
          else if (AvgFlowCalc() >= 5901 && AvgFlowCalc() <= 7900)
            selectorProperty.ChannelHeight = 1600;
          else if (AvgFlowCalc() >= 7901 && AvgFlowCalc() <= 9900)
            selectorProperty.ChannelHeight = 1800;
          else if (AvgFlowCalc() >= 9901 && AvgFlowCalc() <= 13900)
            selectorProperty.ChannelHeight = 2000;

        //double devided = (WaterLevelCalc(selectorProperty) / (selectorProperty.ChannelHeight) + selectorProperty.FreeBoard);
        double devided = (WaterLevelCalc(selectorProperty) / ChannelGateWidthSelect(selectorProperty)) + selectorProperty.FreeBoard;
        double result = Convert.ToInt32(devided / 100) * 100; //have rounded number
        return result;
      }
      catch (ArgumentOutOfRangeException ex)
      {
        throw new ArgumentOutOfRangeException($"Avg flow is out of range,{ex}");
      }
    }
  }
}
