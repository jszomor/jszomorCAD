using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EquipmentPosition
{
  public class PositionProperty
  {
    public double X { get; set; }
    public double Y { get; set; }
    public double Z { get; set; }
    public double ChamberWidth { get; set; }
    public static double NumberOfPump { get; set; }
    public static double DistanceOfPump { get; set; }


    public void DistanceMethod(int distance)
    {
      X = distance;
    }
  }
}
