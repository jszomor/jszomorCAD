using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EquipmentPosition
{
  public class SelectorProperty
  {
    public long AvgDailyFlow { get; set; }
    public long AvgHourlyFlow { get; set; }
    public long Capacity { get; set; }
    public double Head { get; set; }
    public long NumberOfEqipment { get; set; }
    public bool EqPump { get; set; }
    public bool EqLIT { get; set; }
    public bool EqFIT { get; set; }
    public bool EqJet { get; set; }
    public bool EqMixer { get; set; }

    //public SelectorProperty(long numberOfEqipment, long avgDailyFlow, long avgHourlyFlow, long capacity)
    //{
    //  NumberOfEqipment = numberOfEqipment;
    //  AvgDailyFlow = avgDailyFlow;
    //  AvgHourlyFlow = avgHourlyFlow;
    //  Capacity = capacity;
    //}

    
  }
}
