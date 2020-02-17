using JsonFindKey;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EquipmentPosition
{
  public class EquipmentSelector
  {
    public static double AvgFlowCalc()
    {
      SelectorProperty selectorProperty = new SelectorProperty();

      selectorProperty.AvgDailyFlow = JsonProcessClass.JsonProcessValue("Q_inf_AA");

      selectorProperty.AvgHourlyFlow = selectorProperty.AvgDailyFlow / 24;

      return selectorProperty.AvgHourlyFlow;
    }

    public static double EqPumpNumberSelect(SelectorProperty selectorProperty)
    {
      try
      {
        if (AvgFlowCalc() <= 800)
        {
          selectorProperty.NumberOfEqipment = 1;
        }
        else
        {
          switch (AvgFlowCalc())
          {
            case 1 when (AvgFlowCalc() >= 801 && AvgFlowCalc() <= 1200):    
              selectorProperty.Capacity = 800;
              break;

            case 2 when (AvgFlowCalc() >= 1201 && AvgFlowCalc() <= 2000):
              selectorProperty.Capacity = 1000;
              break;

            case 3 when (AvgFlowCalc() >= 2001 && AvgFlowCalc() <= 4000):
              selectorProperty.Capacity = 1200;
              break;

            case 4 when (AvgFlowCalc() >= 4001 && AvgFlowCalc() <= 6000):
              selectorProperty.Capacity = 1500;
              break;

            case 5 when (AvgFlowCalc() >= 6001 && AvgFlowCalc() <= 8500):
              selectorProperty.Capacity = 2000;
              break;
          }
          selectorProperty.NumberOfEqipment = Convert.ToInt32(AvgFlowCalc() / selectorProperty.Capacity);          
        }        
      }
      catch (ArgumentOutOfRangeException ex)
      {
        throw new ArgumentOutOfRangeException($"Avg flow is out of range,{ex}");
      }
      return selectorProperty.NumberOfEqipment;      
    }

    public static double EqPumpFlowCalc(SelectorProperty selectorProperty) => AvgFlowCalc() / EqPumpNumberSelect(selectorProperty);

    public static int ChannelGateWidthSelect(SelectorProperty selectorProperty)
    {
      try
      {
        switch (AvgFlowCalc())
        {
          case 1 when (AvgFlowCalc() <= 900):
            selectorProperty.ChannelWidth = 600;
            break;

          case 2 when (AvgFlowCalc() >= 901 && AvgFlowCalc() <= 1900):
            selectorProperty.ChannelWidth = 1000;
            break;

          case 3 when (AvgFlowCalc() >= 1901 && AvgFlowCalc() <= 3900):
            selectorProperty.ChannelWidth = 1200;
            break;

          case 4 when (AvgFlowCalc() >= 3901 && AvgFlowCalc() <= 5900):
            selectorProperty.ChannelWidth = 1500;
            break;

          case 5 when (AvgFlowCalc() >= 5901 && AvgFlowCalc() <= 7900):
            selectorProperty.ChannelWidth = 1600;
            break;

          case 6 when (AvgFlowCalc() >= 7901 && AvgFlowCalc() <= 9900):
            selectorProperty.ChannelWidth = 1800;
            break;

          case 7 when (AvgFlowCalc() >= 9901 && AvgFlowCalc() <= 13900):
            selectorProperty.ChannelWidth = 2000;
            break;
        }
        return selectorProperty.ChannelWidth;
      }
      catch (ArgumentOutOfRangeException ex)
      {
        throw new ArgumentOutOfRangeException($"Avg flow is out of range,{ex}");
      }
    }

    public static int ChannelGateHeightSelect(SelectorProperty selectorProperty)
    {
      try
      {
        switch (AvgFlowCalc())
        {
          case 1 when (AvgFlowCalc() <= 1900):
            selectorProperty.ChannelHeight = 800;
            break;

          case 3 when (AvgFlowCalc() >= 1901 && AvgFlowCalc() <= 3900):
            selectorProperty.ChannelHeight = 1200;
            break;

          case 4 when (AvgFlowCalc() >= 3901 && AvgFlowCalc() <= 5900):
            selectorProperty.ChannelHeight = 1500;
            break;

          case 5 when (AvgFlowCalc() >= 5901 && AvgFlowCalc() <= 7900):
            selectorProperty.ChannelHeight = 1600;
            break;

          case 6 when (AvgFlowCalc() >= 7901 && AvgFlowCalc() <= 9900):
            selectorProperty.ChannelHeight = 1800;
            break;

          case 7 when (AvgFlowCalc() >= 9901 && AvgFlowCalc() <= 13900):
            selectorProperty.ChannelHeight = 2000;
            break;
        }
        return Convert.ToInt32((AvgFlowCalc()/selectorProperty.NumberOfTrain/3600/0.3/1000000)/(selectorProperty.ChannelHeight + 500));
      }
      catch (ArgumentOutOfRangeException ex)
      {
        throw new ArgumentOutOfRangeException($"Avg flow is out of range,{ex}");
      }
    }
  }
}
