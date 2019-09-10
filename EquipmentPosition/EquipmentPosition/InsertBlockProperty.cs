using Autodesk.AutoCAD.DatabaseServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EquipmentPosition
{
  public class Position
  {
    public double X { get; set; }
    public double Y { get; set; }

    public Position(double x, double y)
    {
      X = x;
      Y = y;
    }
  }

  public class ProcessVariables
  {
    public int NumberOfEqTanks { get; }
  }

  public class InsertBlockBase
  {
    public double X => Position.X;
    public double Y => Position.Y;

    public Position Position { get; set; }
    public double NumberOfItem { get; set; }
    public string BlockName { get; set; }
    public string LayerName { get; set; }
    public string HostName { get; set; }
    public double PipeLength { get; set; }
    public double OffsetX { get; set; }
    public double OffsetY { get; set; }
    public double Rotation { get; set; }
    public object EqIndex { get; set; }
    public IEnumerable<Action<DynamicBlockReferenceProperty>> ActionToExecuteOnDynProp { get; set; }
    public IEnumerable<Action<AttributeReference>> ActionToExecuteOnAttRef { get; set; }
    public IEnumerable<Action<DynamicBlockReferenceProperty>> ActionToExecuteOnDynPropAfter { get; set; }   
    public InsertBlockBase(object eqindex, double numberOfItem, string blockName, string layerName, double x, double y, double rotation, string hostName)
    {
      EqIndex = eqindex;
      Position = new Position(x, y);
      NumberOfItem = numberOfItem;
      BlockName = blockName;
      LayerName = layerName;
      HostName = hostName;
      Rotation = rotation;
    }
  }
}
