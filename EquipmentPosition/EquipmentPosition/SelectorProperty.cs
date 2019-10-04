using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EquipmentPosition
{
  class SelectorProperty
  {
    public double avgDailyFlow { get; set; }
    public double capacity { get; set; }
    public double head { get; set; }
    public double numberOfEqipment { get; set; }
    public bool eqPump { get; set; }
    public bool eqLIT { get; set; }
    public bool eqFIT { get; set; }
    public bool eqJet { get; set; }
    public bool eqMixer { get; set; }
    
  }
}
