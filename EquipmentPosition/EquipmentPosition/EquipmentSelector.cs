using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EquipmentPosition
{
  class EquipmentSelector
  {
    public void EqPumpSelect()
    {
      var selectorProperty = new SelectorProperty();

      selectorProperty.numberOfEqipment = (selectorProperty.avgDailyFlow / 24) / selectorProperty.capacity;
      
    }
  }
}
