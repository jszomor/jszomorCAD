using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EquipmentPosition
{
  public class ProcessVariables
  {
    public int NumberOfEqTanks { get; }
  }

  public class InsertBlockBase
  {
    public double X { get; set; }
    public double Y { get; set; }
    public double NumberOfItem { get; set; }
    public string BlockName { get; set; }
    public string LayerName { get; set; }
    public string PropertyName { get; set; }
    public string HostName { get; set; }
    public double Distance { get; set; }
    public double PipeLength { get; set; }
    public object EqIndex { get; set; }

    public InsertBlockBase(double numberOfItem, double distance, string blockName, string layerName, string propertyName, object eqIndex, double x, double y,
       string hostName, double pipeLength)
    {
      X = x;
      Y = y;
      NumberOfItem = numberOfItem;
      BlockName = blockName;
      LayerName = layerName;
      PropertyName = propertyName;
      HostName = hostName;
      Distance = distance;
      PipeLength = pipeLength;
      EqIndex = eqIndex;
    }
  }
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
}
