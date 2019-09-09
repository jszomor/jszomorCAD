using System.Collections.Generic;

namespace EquipmentPosition
{
  #region
  //public class InsertBlockProperty
  //{
  //  public double Distance { get; set; }
  //  public double PipeLength { get; set; }
  //  public object EqIndex { get; set; }

  //  public InsertBlockProperty(double distance, double pipeLength, object eqIndex)
  //  {
  //    Distance = distance;
  //    PipeLength = pipeLength;
  //    EqIndex = eqIndex;
  //  }
  //}  
  #endregion

  public abstract class EquipmentBase
  {
    public abstract IEnumerable<InsertBlockBase> Blocks { get; }
    public abstract string EquipmentName { get; }
  }
}
