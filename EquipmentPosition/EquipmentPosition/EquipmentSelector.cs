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
    public static long EqPumpSelect()
    {
      SelectorProperty selectorProperty = new SelectorProperty();

      selectorProperty.AvgDailyFlow = JsonProcessClass.JsonProcessValue("Q_inf_AA");

      selectorProperty.AvgHourlyFlow = selectorProperty.AvgDailyFlow / 24;

      try
      {
        if (selectorProperty.AvgHourlyFlow >= 0 && selectorProperty.AvgHourlyFlow <= 400)
        {
          selectorProperty.Capacity = 400;
        }
        else if (selectorProperty.AvgHourlyFlow >= 401 || selectorProperty.AvgHourlyFlow <= 800)
        {
          selectorProperty.Capacity = 600;
        }
        else if (selectorProperty.AvgHourlyFlow >= 801 || selectorProperty.AvgHourlyFlow <= 1200)
        {
          selectorProperty.Capacity = 800;
        }
        else if (selectorProperty.AvgHourlyFlow >= 1201 || selectorProperty.AvgHourlyFlow <= 2000)
        {
          selectorProperty.Capacity = 1000;
        }
        else if (selectorProperty.AvgHourlyFlow >= 2001 || selectorProperty.AvgHourlyFlow <= 4000)
        {
          selectorProperty.Capacity = 1200;
        }
        else if (selectorProperty.AvgHourlyFlow >= 4001 || selectorProperty.AvgHourlyFlow <= 6000)
        {
          selectorProperty.Capacity = 1500;
        }
        else if (selectorProperty.AvgHourlyFlow >= 6001 || selectorProperty.AvgHourlyFlow <= 8500)
        {
          selectorProperty.Capacity = 2000;
        }
      }
      catch (Exception)
      {
        Console.WriteLine("out of range");
      }

      selectorProperty.NumberOfEqipment = selectorProperty.AvgHourlyFlow / selectorProperty.Capacity;
      //selectorProperty.NumberOfEqipment = selectorProperty.AvgHourlyFlow / 40;
      return selectorProperty.NumberOfEqipment;      
    }
  }
}
