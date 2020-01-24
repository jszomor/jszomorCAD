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
    private double? x;
    private double? y;
    private EquipmentStateProperty equipmentStateProperty;

    public double X => Position.X;
    public double Y => Position.Y;

    public Position Position { get; set; }
    public long NumberOfItem { get; set; }
    public string BlockName { get; set; }
    public string LayerName { get; set; }
    public string HostName { get; set; }
    public double PipeLength { get; set; }
    public double OffsetX { get; set; }
    public double OffsetY { get; set; }
    public double Rotation { get; set; }
    public EquipmentStateProperty StateProperty { get; }    
    public IEnumerable<Action<AttributeReference>> ActionToExecuteOnAttRef { get; set; }
    public IEnumerable<Action<DynamicBlockReferenceProperty>> ActionToExecuteOnDynPropAfter { get; set; }

    public InsertBlockBase(long numberOfItem,
      string blockName, string layerName, double x, double y, string hostName,
      EquipmentStateProperty equipmentStateProperty, double rotation = 0.0d)
    {
      Position = new Position(x, y);
      NumberOfItem = numberOfItem;
      BlockName = blockName;
      LayerName = layerName;
      HostName = hostName;
      Rotation = rotation;
      StateProperty = equipmentStateProperty;
    }

    public InsertBlockBase(int numberOfItem, string blockName, string layerName, double? x, double? y, double rotation, EquipmentStateProperty equipmentStateProperty)
    {
      NumberOfItem = numberOfItem;
      BlockName = blockName;
      LayerName = layerName;
      this.x = x;
      this.y = y;
      Rotation = rotation;
      this.equipmentStateProperty = equipmentStateProperty;
    }

    public InsertBlockBase(string blockName)
    {
      BlockName = blockName;
    }
  }

  public class EquipmentStateProperty
  {
    public string PropertyName { get; }
    public object Value { get; }

    public EquipmentStateProperty(string propertyName, object value)
    {
      PropertyName = propertyName;
      Value = value;
    }
  }
}
