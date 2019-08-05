using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EquipmentPosition
{
  public static class DegreeHelper
  {
    public static double rotate0 = 0;
    public static double rotate90 = DegreeToRadian(90);
    public static double rotate180 = DegreeToRadian(180);
    public static double rotate270 = DegreeToRadian(270);

    public static double DegreeToRadian(double angle) =>
      Math.PI * angle / 180.0;

    private static double RadianToDegree(double angle) => 
      angle * (180.0 / Math.PI);
  }  
}
