using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EquipmentPosition
{
  public class SelectorProperty
  {
    public int NumberOfTrain { get; set; }
    public double AvgDailyFlow { get; set; }
    public double AvgHourlyFlow { get; set; }
    public int Capacity { get; set; }
    public int Head { get; set; }
    public int NumberOfEqipment { get; set; }
    public bool EqPump { get; set; }
    public bool EqLIT { get; set; }
    public bool EqFIT { get; set; }
    public bool EqJet { get; set; }
    public bool EqMixer { get; set; }
    public int ChannelWidth { get; set; }
    public int ChannelHeight { get; set; }
    public int FreeBoard { get; set; } = 500;
    public int WaterLevel { get; set; }
    public double FlowSpeed { get; set; } = 0.3;


    //public SelectorProperty(long numberOfEqipment, long avgDailyFlow, long avgHourlyFlow, long capacity)
    //{
    //  NumberOfEqipment = numberOfEqipment;
    //  AvgDailyFlow = avgDailyFlow;
    //  AvgHourlyFlow = avgHourlyFlow;
    //  Capacity = capacity;
    //}


  }
}
