using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EquipmentPosition
{
  public class PositionProperty
  {
    public int X { get; set; }
    public int Y { get; set; }
    public int Z { get; set; }

    public void DistanceMethod(int distance)
    {
      X = distance;
    }
  }
}
