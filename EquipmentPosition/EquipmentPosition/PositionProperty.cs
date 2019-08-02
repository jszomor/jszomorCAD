using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EquipmentPosition
{
  public class PositionProperty
  {
    public const double RAD = 0.01745329252;
    public static double rotate0 = 0;
    public static double rotate90 = RAD * 90;
    public static double rotate180 = RAD * 180;
    public static double rotate270 = RAD * 270;

    private int Distance { get; set; }

    public void EquipmentDist(int eqDistance)
    {
      Distance = eqDistance;
    }
  }
}
